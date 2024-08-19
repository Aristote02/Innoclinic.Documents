using Documents.BusinessLogic.Models;
using Microsoft.AspNetCore.Http;

namespace Documents.BusinessLogic.Services.Interfaces;

/// <summary>
/// Interface for document-related operations
/// </summary>
public interface IDocumentService
{
	/// <summary>
	/// Uploads a PDF file to Blob Storage using the appointment result id
	/// </summary>
	/// <param name="appointmentResultId">The appointment result id</param>
	/// <param name="pdfFile">The byte array of the PDF file</param>
	/// <returns></returns>
	Task UploadPdfAsync(string appointmentResultId, byte[] pdfFile);

	/// <summary>
	/// Retrieves a document from Blob Storage by its documentUrl
	/// </summary>
	/// <param name="documentUrl">The document documentUrl</param>
	/// <returns>DocumentDto containing the document's content and content type</returns>
	Task<DocumentDto> GetDocumentByUrlAsync(string documentUrl);

	/// <summary>
	/// Uploads or updates a document in Blob Storage using an IFormFile
	/// </summary>
	/// <param name="file">The file to be uploaded or updated</param>
	/// <returns></returns>
	Task UploadOrUpdateDocumentByUrlAsync(IFormFile file);

	/// <summary>
	/// Deletes a document from Blob Storage using its url
	/// </summary>
	/// <param name="documentUrl">The document url</param>
	Task DeleteDocumentByUrlAsync(string documentUrl);
}