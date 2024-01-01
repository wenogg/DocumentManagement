using Elsa;

var builder = WebApplication.CreateBuilder(args);

var elsaSection = builder.Configuration.GetSection("Elsa");
builder.Services
    .AddElsa(elsa => elsa
        //.UseEntityFrameworkPersistence(ef => ef.UseSqlite())
        .AddConsoleActivities()
        .AddHttpActivities(elsaSection.GetSection("Server").Bind)
        .AddQuartzTemporalActivities()
        //.AddWorkflowsFrom<Startup>()
    );

builder.Services.AddRazorPages();

var app = builder.Build();

app
    .UseStaticFiles()
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
        endpoints.MapControllers();

        // For Dashboard.
        endpoints.MapFallbackToPage("/_Host");
    })
    .UseWelcomePage();

app.Run();