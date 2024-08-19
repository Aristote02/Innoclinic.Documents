namespace Documents.BusinessLogic.Configurations;

/// <summary>
/// Represents the configuration settings for Blob Storage
/// </summary>
public class BlobStorageConfigurations
{
	/// <summary>
	/// Gets or initializes the connection string for the Blob Storage account
	/// </summary>
	public required string ConnectionString { get; init; }

	/// <summary>
	/// Gets or initializes the name of the Blob Storage container
	/// </summary>
	public required string ContainerName { get; init; }

	/// <summary>
	/// Gets or initializes the primary key for accessing the Blob Storage account
	/// </summary>
	public required string PrimaryKey { get; init; }

	/// <summary>
	/// Gets or initializes the name of the Blob Storage account
	/// </summary>
	public required string AccountName { get; init; }
}