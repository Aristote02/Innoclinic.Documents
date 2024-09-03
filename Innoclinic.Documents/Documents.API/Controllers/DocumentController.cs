using Documents.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Documents.API.Controllers;

[Route("api/documents")]
[ApiController]
public class DocumentController : ControllerBase
{
	private readonly IDocumentService _documentService;

	/// <summary>
	/// Initializes an instance of <see cref="DocumentController"/>
	/// </summary>
	/// <param name="documentService">The document service instance</param>
	public DocumentController(IDocumentService documentService)
	{
		_documentService = documentService;
	}

	#region GetDocumentByUrl
	/// <summary>
	/// Retrieves a specific document by its url
	/// </summary>
	/// <param name="documentUrl">The document documentUrl</param>
	/// <remarks>
	/// Sample request:
	/// 
	///		GET api/documents/document/{documentUrl}
	/// </remarks>
	/// <returns>File content with content type</returns>
	/// <response code="200">Returns the document successfully</response>
	/// <response code="404">If the document is not found</response>
	#endregion
	[HttpGet("document/{documentUrl}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetDocumentByUrl(string documentUrl)
	{
		var documentRetrieved = await _documentService.GetDocumentByUrlAsync(documentUrl);

		return File(documentRetrieved.Content, documentRetrieved.ContentType);
	}

	#region UploadOrUpdateDocumentByUrl
	/// <summary>
	/// Uploads or updates a document in Blob Storage
	/// </summary>
	/// <param name="file">The file to be uploaded or updated</param>
	/// <remarks>
	/// Sample request:
	/// 
	///		POST api/documents/document
	///		Content-Type: multipart/form-data
	///		Form data: file=[File to upload]
	/// </remarks>
	/// <returns>A status message indicating success</returns>
	/// <response code="200">If the file is uploaded or updated successfully</response>
	/// <response code="500">If there is an error during the upload or update process</response>
	#endregion
	[HttpPost("document")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UploadOrUpdateDocumentByUrl(IFormFile file)
	{
		await _documentService.UploadOrUpdateDocumentByUrlAsync(file);

		return Ok("File uploaded");
	}

	#region DeleteDocumentByUrl
	/// <summary>
	/// Deletes a document from Blob Storage using its url
	/// </summary>
	/// <param name="documentUrl">The document url</param>
	/// <remarks>
	/// Sample request:
	/// 
	///		DELETE api/documents/document/{documentUrl}
	/// </remarks>
	/// <returns>A status message indicating success</returns>
	/// <response code="200">If the file is deleted successfully</response>
	/// <response code="404">If the file is not found</response>
	/// <response code="500">If there is an error during the deletion process</response>
	#endregion
	[HttpDelete("document/{documentUrl}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> DeleteDocumentByUrl(string documentUrl)
	{
		await _documentService.DeleteDocumentByUrlAsync(documentUrl);

		return Ok("File deleted");
	}
}