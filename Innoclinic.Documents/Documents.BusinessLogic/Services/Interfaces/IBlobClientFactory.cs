using Azure.Storage.Blobs;

namespace Documents.BusinessLogic.Services.Interfaces;

/// <summary>
/// Interface for the BlobClientFactory
/// </summary>
public interface IBlobClientFactory
{
	/// <summary>
	///  Creates a new BlobClient instance for the specified blob URI
	/// </summary>
	/// <param name="blobUri">The URI of the blob</param>
	/// <returns>A new BlobClient instance</returns>
	BlobClient CreateBlobClient(string blobUri);
}
