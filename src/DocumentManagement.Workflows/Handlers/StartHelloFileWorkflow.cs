using DocumentManagement.Core.Documents;
using Elsa.Models;
using Elsa.Services;
using MediatR;

namespace DocumentManagement.Workflows.Handlers;

public class StartHelloFileWorkflow(
    IWorkflowRegistry workflowRegistry,
    IWorkflowDefinitionDispatcher workflowDispatcher)
    : INotificationHandler<NewDocumentReceived>
{
    public async Task Handle(NewDocumentReceived notification, CancellationToken cancellationToken)
    {
        var document = notification.Document;

        // Get our HelloFile workflow.
        var workflow = await workflowRegistry.FindByNameAsync("HelloFile", VersionOptions.Published, cancellationToken: cancellationToken);

        if (workflow == null)
            return; // Do nothing.

        // Dispatch the workflow.
        await workflowDispatcher.DispatchAsync(new ExecuteWorkflowDefinitionRequest(workflow.Id, Input: new WorkflowInput(document.Id)), cancellationToken);
    }
}