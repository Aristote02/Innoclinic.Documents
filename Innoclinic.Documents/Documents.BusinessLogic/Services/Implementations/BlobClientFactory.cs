using Azure.Storage;
using Azure.Storage.Blobs;
using Documents.BusinessLogic.Configurations;
using Documents.BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Documents.BusinessLogic.Services.Implementations;

/// <summary>
/// Factory class to create BlobClient instances
/// </summary>
public class BlobClientFactory : IBlobClientFactory
{
	/// <summary>
	/// Configuration options for Blob Storage
	/// </summary>
	private readonly BlobStorageConfigurations _options;

	/// <summary>
	/// Initialize the BlobClientFactory with BlobStorageConfigurations options
	/// </summary>
	/// <param name="options">The Blob Storage configuration options</param>
	public BlobClientFactory(IOptions<BlobStorageConfigurations> options)
	{
		_options = options.Value;
	}

	/// <summary>
	/// Creates a new BlobClient instance for the specified blob URI
	/// </summary>
	/// <param name="blobUri">The URI of the blob</param>
	/// <returns>A new BlobClient instance</returns>
	public BlobClient CreateBlobClient(string blobUri)
	{
		return new BlobClient(new Uri(blobUri), new StorageSharedKeyCredential(_options.AccountName, _options.PrimaryKey));
	}
}
