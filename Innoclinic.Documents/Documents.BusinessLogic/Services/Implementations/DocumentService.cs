using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Documents.BusinessLogic.Configurations;
using Documents.BusinessLogic.Models;
using Documents.BusinessLogic.Services.Interfaces;
using Innoclinic.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Documents.BusinessLogic.Services.Implementations;

/// <summary>
/// Represents the Service implementation for document-related operations
/// </summary>
public class DocumentService : IDocumentService
{
	private readonly BlobStorageConfigurations _options;
	private readonly BlobContainerClient _blobContainerClient;
	private readonly IBlobClientFactory _blobClientFactory;
	private readonly ILogger<DocumentService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="DocumentService"/> class
	/// </summary>
	/// <param name="blobService">The BlobServiceClient instance</param>
	/// <param name="options">The Blob Storage configuration options</param>
	/// <param name="logger">The logger instance</param>
	/// <param name="blobClientFactory">The BlobClientFactory instance</param>
	public DocumentService(BlobServiceClient blobService,
		IOptions<BlobStorageConfigurations> options,
		ILogger<DocumentService> logger,
		IBlobClientFactory blobClientFactory)
	{
		_options = options.Value;
		_blobContainerClient = blobService.GetBlobContainerClient(_options.ContainerName);
		_blobContainerClient.CreateIfNotExists();
		_logger = logger;
		_blobClientFactory = blobClientFactory;
	}

	/// <summary>
	/// Retrieves a document from Blob Storage by its documentUrl
	/// </summary>
	/// <param name="documentUrl">The document documentUrl</param>
	/// <returns>DocumentDto containing the document's content and content type</returns>
	/// <exception cref="NotFoundException">Thrown when the document is not found</exception>
	public async Task<DocumentDto> GetDocumentByUrlAsync(string documentUrl)
	{
		var blobClient = _blobClientFactory.CreateBlobClient($"{_blobContainerClient.Uri}/{documentUrl}");
		_logger.LogInformation("Attempting to retrieve blob with name: {blobName}", documentUrl);

		try
		{
			var downloadInfo = await blobClient.DownloadAsync();
			var contentType = downloadInfo.Value.Details.ContentType;

			return new DocumentDto
			{
				Content = downloadInfo.Value.Content,
				ContentType = downloadInfo.Value.Details.ContentType
			};
		}
		catch (RequestFailedException exception) when (exception.Status == 404)
		{
			_logger.LogError("The file with the given name is not found");

			throw new NotFoundException("No file with the given name is not found");
		}
	}

	/// <summary>
	/// Uploads or updates a document in Blob Storage using an IFormFile
	/// </summary>
	/// <param name="file">The file to be uploaded or updated</param>
	public async Task UploadOrUpdateDocumentByUrlAsync(IFormFile file)
	{
		try
		{
			var blobClient = _blobClientFactory.CreateBlobClient($"{_blobContainerClient.Uri}/{file}");

			await blobClient.UploadAsync(file.OpenReadStream(), new BlobUploadOptions
			{
				HttpHeaders = new BlobHttpHeaders
				{
					ContentType = file.ContentType
				}
			});

			_logger.LogInformation("The file has been uploaded or updated successfully");
		}
		catch (RequestFailedException exception)
		{
			_logger.LogError("A error occured while trying to upload or update the file, Message:{message}", exception.Message);

			throw;
		}
	}

	/// <summary>
	/// Deletes a document from Blob Storage using its url
	/// </summary>
	/// <param name="documentUrl">The document url</param>
	/// <exception cref="NotFoundException">Thrown when the document is not found</exception>
	public async Task DeleteDocumentByUrlAsync(string documentUrl)
	{
		try
		{
			var blobClient = _blobClientFactory.CreateBlobClient($"{_blobContainerClient.Uri}/{documentUrl}");

			await blobClient.DeleteAsync();

			_logger.LogInformation("The file was successully deleted");
		}
		catch (RequestFailedException exception) when (exception.Status == 404)
		{
			_logger.LogError("An error occured while trying to delete the file, Message:{message}", exception.Message);

			throw new NotFoundException("The file does not exist");
		}
	}

	/// <summary>
	/// Uploads a PDF file to Blob Storage using the appointment result id
	/// </summary>
	/// <param name="appointmentResultId">The appointment result id</param>
	/// <param name="pdfFile">The byte array of the PDF file</param>
	public async Task UploadPdfAsync(string appointmentResultId, byte[] pdfFile)
	{
		var blobClient = _blobClientFactory.CreateBlobClient($"{_blobContainerClient.Uri}/{appointmentResultId}");
		using var stream = new MemoryStream(pdfFile);
		await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = "application/pdf" });
		_logger.LogInformation("PDF uploaded to blob storage successfully: {documentUrl}", appointmentResultId);
	}
}