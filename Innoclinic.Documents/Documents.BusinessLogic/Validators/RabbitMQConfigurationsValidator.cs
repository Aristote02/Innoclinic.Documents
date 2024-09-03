using FluentValidation;
using Innoclinic.Shared.MessageBrokers.Configurations;

namespace Documents.BusinessLogic.Validators;

/// <summary>
///  Validator for RabbitMQConfigurations using FluentValidation
/// </summary>
public class RabbitMQConfigurationsValidator : AbstractValidator<RabbitMQConfigurations>
{
	/// <summary>
	/// Sets validation rules
	/// </summary>
	public RabbitMQConfigurationsValidator()
	{
		RuleFor(x => x.Host)
			.NotEmpty().WithMessage("RabbitMQ Host is required");

		RuleFor(x => x.UserName)
			.NotEmpty().WithMessage("RabbitMQ UserName is required");

		RuleFor(x => x.Password)
			.NotEmpty().WithMessage("RabbitMQ Password is required");
	}
}