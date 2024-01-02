using System.Security.Cryptography;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Models;
using Elsa.Providers.WorkflowStorage;
using Elsa.Services;
using Elsa.Services.Models;
using Hangfire;

namespace DocumentManagement.Workflows.Activities;

public record UpdateBlockchainContext(string WorkflowInstanceId, string ActivityId, string FileSignature);

[Activity(
    Category = "Document Management",
    Description = "Saves a hash of the specified file onto the blockchain to prevent tampering."
)]
public class UpdateBlockchain(IBackgroundJobClient backgroundJobClient, IWorkflowInstanceDispatcher workflowInstanceDispatcher) : Activity
{
    [ActivityInput(
        Label = "File",
        Hint = "The file to store its hash of onto the blockchain. Can be byte[] or Stream.",
        SupportedSyntaxes = new[] {SyntaxNames.JavaScript, SyntaxNames.Liquid},
        DefaultWorkflowStorageProvider = TransientWorkflowStorageProvider.ProviderName
    )]
    public required object File { get; set; }

    [ActivityOutput(Hint = "The computed file signature as stored on the blockchain.")]
    public required string Output { get; set; }

    protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
    {
        // Determine the type of File object: is it a Stream or a byte array?
        var bytes = File switch
        {
            Stream stream => await stream.ReadBytesToEndAsync(),
            byte[] buffer => buffer,
            _ => throw new NotSupportedException()
        };

        // Compute hash.
        var fileSignature = ComputeSignature(bytes);

        backgroundJobClient.Enqueue(() => SubmitToBlockChainAsync(new UpdateBlockchainContext(context.WorkflowInstance.Id, context.ActivityId, fileSignature), CancellationToken.None));

        // Suspend the workflow.
        return Suspend();
    }

    /// <summary>
    /// Invoked by Hangfire as a background job.
    /// </summary>
    public async Task SubmitToBlockChainAsync(UpdateBlockchainContext context, CancellationToken cancellationToken)
    {
        // Simulate storing it on an imaginary blockchain out there.
        await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);

        // Resume the suspended workflow.
        var workflowInstanceRequest = new ExecuteWorkflowInstanceRequest(
            context.WorkflowInstanceId,
            context.ActivityId,
            new WorkflowInput(context.FileSignature));

        await workflowInstanceDispatcher.DispatchAsync(workflowInstanceRequest, cancellationToken);
    }

    protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
    {
        // When we resume, read the simply complete this activity.
        var fileSignature = context.GetInput<string>();
        Output = fileSignature!;
        return Done();
    }

    private static string ComputeSignature(byte[] bytes)
    {
        using var algorithm = SHA256.Create();
        var hashValue = algorithm.ComputeHash(bytes);
        return Convert.ToBase64String(hashValue);
    }
}