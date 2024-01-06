namespace DocumentManagement.Core.Documents;

public enum DocumentStatus
{
    New,
    Archived,

    // Leave Request
    PendingApproval,
    Approved,
    Rejected,

    // Use activation
    PendingActivation,
    Activated,
    ActivationCancelled,

    // Use activation
    Compiling,
    Shipping,
    Shipped,
    ShipmentCancelled
}