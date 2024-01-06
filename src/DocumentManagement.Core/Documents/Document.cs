using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagement.Core.Documents;

public class Document
{
    public string Id { get; set; } = default!;

    public string DocumentTypeId { get; set; } = default!;

    public DocumentType DocumentType { get; set; } = default!;

    public string FileName { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public string? Notes { get; set; }

    public DocumentStatus Status { get; set; }

    [NotMapped]
    public string[] Actions { get; set; } = [];
}