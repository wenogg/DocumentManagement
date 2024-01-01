using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using DocumentManagement.Persistence.Extensions;
using DocumentManagement.Persistence.Files;
using DocumentManagement.Web.Components;
using DocumentManagement.Web.Extensions;
using DocumentManagement.Workflows.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;
using Elsa.Server.Hangfire.Extensions;
using Hangfire;
using Hangfire.SQLite;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Storage.Net;

var builder = WebApplication.CreateBuilder(args);

var dbConnectionString = builder.Configuration.GetConnectionString("Sqlite")!;

// Add services to the container.
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddDomainServices();
builder.Services.Configure<DocumentStorageOptions>(options =>
    options.BlobStorageFactory = () =>
        StorageFactory.Blobs.DirectoryFiles(Path.Combine(Environment.CurrentDirectory, "App_Data/Uploads")));

builder.Services.AddDomainPersistence(dbConnectionString);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddBlazorise( options =>
    {
        options.Immediate = true;
    } )
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();


// Hangfire (for background tasks).
builder.Services
    .AddHangfire(config => config
        // Use same SQLite database as Elsa for storing jobs.
        //.UseSqlServerStorage(dbConnectionString)
        .UseSQLiteStorage(dbConnectionString)
        .UseSimpleAssemblyNameTypeSerializer()
        // Elsa uses NodaTime primitives, so Hangfire needs to be able to serialize them.
        .UseRecommendedSerializerSettings(settings => settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb)))
    .AddHangfireServer((sp, options) =>
    {
        // Bind settings from configuration.
        builder.Configuration.GetSection("Hangfire").Bind(options);
        // Configure queues for Elsa workflow dispatchers.
        options.ConfigureForElsaDispatchers(sp);
    });

builder.Services.AddElsa(
    builder.Configuration,
    ef => ef.UseSqlite(dbConnectionString));

builder.Services.AddWorkflowServices();

builder.Services
    .AddElsaSwagger()
    .AddElsaApiEndpoints();

// Allow arbitrary client browser apps to access the API for demo purposes only.
// In a production environment, make sure to allow only origins you trust.
builder.Services.AddCors(cors =>
    cors.AddDefaultPolicy(policy =>
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()
            .WithExposedHeaders("Content-Disposition")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseRouting();
app.UseHttpActivities();

app.MapControllers();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();