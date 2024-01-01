using DocumentManagement.Core.Documents;
using Elsa.Models;
using Elsa.Services;
using MediatR;

namespace DocumentManagement.Workflows.Handlers;

public class StartDocumentWorkflows(
    IWorkflowRegistry workflowRegistry,
    IWorkflowDefinitionDispatcher workflowDispatcher)
    : INotificationHandler<NewDocumentReceived>
{
    public async Task Handle(NewDocumentReceived notification, CancellationToken cancellationToken)
    {
        var document = notification.Document;
        var documentTypeName = document.DocumentType.Name;

        // Get our HelloFile workflow.
        var workflowBlueprints =
            (await workflowRegistry.FindManyByTagAsync(documentTypeName, VersionOptions.Published, cancellationToken: cancellationToken))
            .ToList();

        if (!workflowBlueprints.Any())
            return;

        // Dispatch the workflow.
        foreach (var workflowBlueprint in workflowBlueprints)
        {
            await workflowDispatcher.DispatchAsync(new ExecuteWorkflowDefinitionRequest(workflowBlueprint.Id, CorrelationId: document.Id, Input: new WorkflowInput(document.Id)), cancellationToken);
        }
    }
}