using System.ComponentModel.DataAnnotations;

namespace DocumentManagement.Core.Documents;

public class CreateDocumentDto
{
    [Required]
    public string DocumentTypeId { get; set; } = default!;

    public string Name { get; set; } = default!;

    public byte[] File { get; set; } = default!;
}