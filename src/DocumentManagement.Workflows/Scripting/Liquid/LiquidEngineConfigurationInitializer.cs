using DocumentManagement.Core.Documents;
using DocumentManagement.Workflows.Activities;
using Elsa.Scripting.Liquid.Messages;
using Fluid;
using MediatR;

namespace DocumentManagement.Workflows.Scripting.Liquid;

public class LiquidEngineConfigurationInitializer : INotificationHandler<EvaluatingLiquidExpression>
{
    public Task Handle(EvaluatingLiquidExpression notification, CancellationToken cancellationToken)
    {
        var memberAccessStrategy = notification.TemplateContext.Options.MemberAccessStrategy;

        memberAccessStrategy.Register<Document>();
        memberAccessStrategy.Register<DocumentFile>();

        return Task.CompletedTask;
    }
}