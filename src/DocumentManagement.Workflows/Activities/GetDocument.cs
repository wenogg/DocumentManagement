using DocumentManagement.Core.Documents;
using DocumentManagement.Core.Files;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Providers.WorkflowStorage;
using Elsa.Services;
using Elsa.Services.Models;

namespace DocumentManagement.Workflows.Activities;

public record DocumentFile(Document Document, Stream FileStream);

[Action(Category = "Document Management", Description = "Gets the specified document from the database.")]
public class GetDocument(IDocumentStore documentStore, IFileStorage fileStorage) : Activity
{
    [ActivityInput(
        Label = "Document ID",
        Hint = "The ID of the document to load",
        SupportedSyntaxes = [SyntaxNames.JavaScript, SyntaxNames.Liquid]
    )]
    public string DocumentId { get; set; } = default!;

    [ActivityOutput(
        Hint = "The document + file.",
        DefaultWorkflowStorageProvider = TransientWorkflowStorageProvider.ProviderName)]
    public DocumentFile Output { get; set; } = default!;

    protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
    {
        var document = await documentStore.Get(DocumentId, context.CancellationToken);
        var fileStream = await fileStorage.Read(document!.FileName, context.CancellationToken);

        Output = new DocumentFile(document, fileStream);
        return Done();
    }
}