using DocumentManagement.Core.Documents;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Providers.WorkflowStorage;
using Elsa.Services;
using Elsa.Services.Models;

namespace DocumentManagement.Workflows.Activities;

[Activity(Category = "Document Management", Description = "Archives the specified document.")]
public class ArchiveDocument(IDocumentStore documentStore) : Activity
{
    [ActivityInput(
        Label = "Document",
        Hint = "The document to archive",
        SupportedSyntaxes = [SyntaxNames.JavaScript, SyntaxNames.Liquid],
        DefaultWorkflowStorageProvider = TransientWorkflowStorageProvider.ProviderName
    )]
    public Document Document { get; set; } = default!;

    protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
    {
        Document.Status = DocumentStatus.Archived;
        await documentStore.Save(Document);
        return Done();
    }
}