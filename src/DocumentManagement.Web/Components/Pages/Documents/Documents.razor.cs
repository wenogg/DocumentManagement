using Blazorise;
using DocumentManagement.Core.Documents;
using Microsoft.AspNetCore.Components;


namespace DocumentManagement.Web.Components.Pages.Documents;

public partial class Documents
{
    [Inject]
    private IDocumentTypeStore DocumentTypeStore { get; set; } = default!;

    [Inject]
    private IDocumentService DocumentService { get; set; } = default!;

    private List<DocumentType> DocumentTypes { get; set; } = [];

    private CreateDocumentDto CreateDocumentDto { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        DocumentTypes = (await DocumentTypeStore.List()).ToList();
        await base.OnInitializedAsync();
    }

    private async Task OnDocumentChanged(FileChangedEventArgs arg)
    {
        var file = arg.Files.FirstOrDefault();
        if (file == null)
        {
            return;
        }

        using var stream = new MemoryStream();
        await file.WriteToStreamAsync(stream);
        stream.Seek(0, SeekOrigin.Begin);

        var extension = Path.GetExtension(file.Name);
        var fileName = $"{Guid.NewGuid()}{extension}";

        CreateDocumentDto.File = stream.GetBuffer();
        CreateDocumentDto.Name = fileName;
    }

    public async Task SaveDocument()
    {
        using var stream = new MemoryStream();
        await stream.WriteAsync(CreateDocumentDto.File);
        stream.Seek(0, SeekOrigin.Begin);
        var document = await DocumentService.SaveDocumentAsync(CreateDocumentDto.Name, stream, CreateDocumentDto.DocumentTypeId);

        // return RedirectToPage("FileReceived", new {DocumentId = document.Id});
        //
        // return Task.CompletedTask;
    }
}