using Elsa.Scripting.JavaScript.Events;
using Elsa.Scripting.JavaScript.Messages;
using MediatR;
using Microsoft.AspNetCore.StaticFiles;

namespace DocumentManagement.Workflows.Scripting.JavaScript;

/// <summary>
/// Registers custom JS functions.
/// </summary>
public class ConfigureJavaScriptEngineWithCustomFunctions(IContentTypeProvider contentTypeProvider) :
    INotificationHandler<EvaluatingJavaScriptExpression>,
    INotificationHandler<RenderingTypeScriptDefinitions>
{
    public Task Handle(EvaluatingJavaScriptExpression notification, CancellationToken cancellationToken)
    {
        var engine = notification.Engine;

        engine.SetValue("contentTypeFromFileName", (Func<string, string>) GetContentType);

        return Task.CompletedTask;
    }

    public Task Handle(RenderingTypeScriptDefinitions notification, CancellationToken cancellationToken)
    {
        var output = notification.Output;

        output.AppendLine("declare function contentTypeFromFileName(fileName: string): string");

        return Task.CompletedTask;
    }

    private string GetContentType(string fileName) =>
        contentTypeProvider.TryGetContentType(fileName, out var contentType)
            ? contentType
            : "application/octet-stream";
}