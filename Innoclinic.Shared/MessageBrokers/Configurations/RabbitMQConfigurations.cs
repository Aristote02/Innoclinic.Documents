namespace Innoclinic.Shared.MessageBrokers.Configurations;

public class RabbitMQConfigurations
{
	public required string Host { get; init; }
	public required string UserName { get; init; }
	public required string Password { get; init; }
}