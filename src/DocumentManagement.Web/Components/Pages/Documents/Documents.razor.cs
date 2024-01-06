using Blazorise;
using DocumentManagement.Core.Documents;
using Elsa;
using Elsa.Activities.Signaling.Services;
using Elsa.Activities.UserTask.Contracts;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Services;
using Elsa.Services.WorkflowStorage;
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

    [Inject]
    private ISignaler WorkflowSignaler { get; set; }

    [Inject] IWorkflowInstanceStore WorkflowInstanceStore { get; set; }

    [Inject] IWorkflowStorageService WorkflowStorageService { get; set; }

    [Inject] IWorkflowTriggerInterruptor WorkflowTriggerInterruptor { get; set; }

    [Inject] IUserTaskService UserTaskService { get; set; }

    private List<DocumentType> DocumentTypes { get; set; } = [];

    private List<Document> Items { get; set; } = [];

    private CreateDocumentDto CreateDocumentDto { get; set; } = new();

    private bool ShowUploadSuccess { get; set; }

    protected override async Task OnInitializedAsync()
    {
        DocumentTypes = (await DocumentTypeStore.List()).ToList();
        Items = await DocumentStore.List();

        foreach (var item in Items)
        {
            var workflowInstance = await WorkflowInstanceStore.FindByCorrelationIdAsync(item.Id);
            if (workflowInstance == null) continue;
            var actions = (await UserTaskService.GetUserActionsAsync(workflowInstance!.Id)).ToList();
            if (actions.Count == 0) continue;

            item.Actions = actions.Select(x => x.Action).ToArray();
        }
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
        await DocumentService.SaveDocumentAsync(CreateDocumentDto.Name, stream, CreateDocumentDto.DocumentTypeId);

        ShowUploadSuccess = true;
        CreateDocumentDto = new CreateDocumentDto();
        Items = await DocumentStore.List();
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

    private async Task ApplyTransition(Document document, string transition)
    {
        await WorkflowSignaler.TriggerSignalAsync(correlationId: document.Id, signal: transition);
    }

    private async Task SendUserAction(Document document, string action)
    {
        var workflowInstance = await WorkflowInstanceStore.FindByCorrelationIdAsync(document.Id);
        var currentActivity = workflowInstance?.BlockingActivities.FirstOrDefault();
        if (currentActivity == null) return;

        await WorkflowStorageService.UpdateInputAsync(workflowInstance, new WorkflowInput(action));
        await WorkflowTriggerInterruptor.InterruptActivityAsync(workflowInstance, currentActivity.ActivityId);
    }
}