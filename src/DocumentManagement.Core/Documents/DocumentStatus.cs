namespace DocumentManagement.Core.Documents;

public enum DocumentStatus
{
    New,
    Archived,
    PendingApproval,
    Approved,
    Rejected,
    PendingActivation,
    Activated,
    ActivationCancelled
}