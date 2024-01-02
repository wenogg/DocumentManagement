using Blazorise;
using DocumentManagement.Core.Documents;
using Elsa.Models;
using Elsa.Services;
using Microsoft.AspNetCore.Components;


namespace DocumentManagement.Web.Components.Pages.Documents;

public partial class Documents
{
    private const string WorkflowName = "Get Document";

    [Inject]
    private IDocumentStore DocumentStore { get; set; }

    [Inject]
    private IDocumentTypeStore DocumentTypeStore { get; set; }

    [Inject]
    private IDocumentService DocumentService { get; set; }

    [Inject]
    private IWorkflowRegistry WorkflowRegistry { get; set; }

    [Inject]
    private IWorkflowDefinitionDispatcher WorkflowDefinitionDispatcher { get; set; }

    private List<DocumentType> DocumentTypes { get; set; } = [];

    private List<Document> Items { get; set; } = [];

    private CreateDocumentDto CreateDocumentDto { get; set; } = new();

    private bool ShowUploadSuccess { get; set; }

    protected override async Task OnInitializedAsync()
    {
        DocumentTypes = (await DocumentTypeStore.List()).ToList();
        Items = (await DocumentStore.List()).ToList();
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

        ShowUploadSuccess = true;
        CreateDocumentDto = new CreateDocumentDto();
    }

    private async Task ArchiveDocument(Document document)
    {
        // Get our HelloFile workflow.
        var workflowBlueprint = await WorkflowRegistry.FindByNameAsync(WorkflowName, VersionOptions.Published);

        if (workflowBlueprint == null)
            return;

        // Dispatch the workflow.
        var executionDefinition = new ExecuteWorkflowDefinitionRequest(
            workflowBlueprint.Id,
            CorrelationId: document.Id,
            Input: new WorkflowInput(document.Id));
        await WorkflowDefinitionDispatcher.DispatchAsync(executionDefinition);
    }
}