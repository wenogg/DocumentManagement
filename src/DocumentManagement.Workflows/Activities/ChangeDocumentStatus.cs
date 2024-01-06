using DocumentManagement.Core.Documents;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Providers.WorkflowStorage;
using Elsa.Services;
using Elsa.Services.Models;

namespace DocumentManagement.Workflows.Activities;

[Activity(Category = "Document Management", Description = "Changes the status of the document.")]
public class ChangeDocumentStatus(IDocumentStore documentStore) : Activity
{
    [ActivityInput(
        Label = "Document ID",
        Hint = "The ID of the document to load",
        SupportedSyntaxes = [SyntaxNames.JavaScript, SyntaxNames.Liquid]
    )]
    public string DocumentId { get; set; } = default!;

    [ActivityInput(
        Label = "Status",
        Hint = "The new Status of the document",
        SupportedSyntaxes = [SyntaxNames.JavaScript, SyntaxNames.Liquid],
        DefaultWorkflowStorageProvider = TransientWorkflowStorageProvider.ProviderName
    )]
    public string DocumentStatus { get; set; } = default!;

    [ActivityOutput(
        Hint = "The new status of the document",
        DefaultWorkflowStorageProvider = TransientWorkflowStorageProvider.ProviderName)]
    public DocumentStatus NewStatus { get; set; }

    protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
    {
        var document = await documentStore.Get(DocumentId, context.CancellationToken);
        NewStatus = Enum.Parse<DocumentStatus>(DocumentStatus);
        document!.Status = NewStatus;
        await documentStore.Save(document);
        return Done();
    }
}