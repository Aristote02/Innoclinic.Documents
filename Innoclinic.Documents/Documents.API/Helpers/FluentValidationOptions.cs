using FluentValidation;
using Microsoft.Extensions.Options;

namespace Documents.API.Helpers;

/// <summary>
/// Implements the IValidateOptions interface, providing validation logic using FluentValidation
/// </summary>
/// <typeparam name="TOptions">The type of the options to validate</typeparam>
public class FluentValidationOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
{
	private readonly IServiceProvider _serviceProvider;
	private readonly string? _name;

	/// <summary>
	/// Initializes a new instance of the <see cref="FluentValidationOptions{TOptions}"/> class
	/// </summary>
	/// <param name="serviceProvider">The service provider for creating validators</param>
	/// <param name="name">The name of the options instance</param>
	public FluentValidationOptions(IServiceProvider serviceProvider, string? name)
	{
		_serviceProvider = serviceProvider;
		_name = name;
	}

	/// <summary>
	/// Validates the options instance using FluentValidation
	/// </summary>
	/// <param name="name">The name of the options instance being validated</param>
	/// <param name="options">The options instance to validate</param>
	/// <returns>The result of the validation</returns>
	public ValidateOptionsResult Validate(string? name, TOptions options)
	{
		if (_name != null && _name != name)
		{
			return ValidateOptionsResult.Skip;
		}

		ArgumentNullException.ThrowIfNull(options);

		using var scope = _serviceProvider.CreateScope();
		var validator = scope.ServiceProvider.GetRequiredService<IValidator<TOptions>>();
		var results = validator.Validate(options);
		if (results.IsValid)
		{
			return ValidateOptionsResult.Success;
		}

		var errors = results.Errors
			.Select(e => $"Fluent validation failed for '{options.GetType().Name}.{e.PropertyName}' with the error: '{e.ErrorMessage}' ")
			.ToList();

		return ValidateOptionsResult.Fail(errors);
	}
}