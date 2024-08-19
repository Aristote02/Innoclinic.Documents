namespace Documents.BusinessLogic.Models;

/// <summary>
///  Represents a Data transfer object for document content and metadata
/// </summary>
public class DocumentDto
{
	/// <summary>
	/// The content of the document as a stream
	/// </summary>
	public required Stream Content { get; init; }

	/// <summary>
	/// The MIME type of the document content
	/// </summary>
	public required string ContentType { get; init; }
}
