using Elsa.Activities.Http.Events;
using MediatR;

namespace DocumentManagement.Web.Handlers;

public class SignalReceivedRedirectHandler(IHttpContextAccessor httpContextAccessor)
    : INotificationHandler<HttpTriggeredSignal>
{
    public Task Handle(HttpTriggeredSignal notification, CancellationToken cancellationToken)
    {
        var affectedWorkflows = notification.AffectedWorkflows;

        // Exit if no workflows were affected.
        if (affectedWorkflows.Count == 0)
            return Task.CompletedTask;

        var signalName = notification.SignalModel.Name;
        var response = httpContextAccessor.HttpContext!.Response;

        // Redirect to the appropriate page.
        switch (signalName)
        {
            case "Approve":
                response.Redirect("/leave-request-approved");
                break;
            case "Reject":
                response.Redirect("/leave-request-denied");
                break;
        }

        return Task.CompletedTask;
    }
}