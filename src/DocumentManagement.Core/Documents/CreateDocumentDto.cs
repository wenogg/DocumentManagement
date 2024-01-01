namespace DocumentManagement.Core.Documents;

public class CreateDocumentDto
{
    public string DocumentTypeId { get; set; } = default!;

    public string Name { get; set; } = default!;

    public byte[] File { get; set; } = default!;
}