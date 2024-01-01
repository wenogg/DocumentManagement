using MediatR;

namespace DocumentManagement.Core.Documents;

public record NewDocumentReceived(Document Document) : INotification;