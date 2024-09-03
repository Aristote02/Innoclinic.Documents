using Documents.API.Helpers;
using Microsoft.Extensions.Options;

namespace Documents.API.Extensions;

/// <summary>
/// This class provides an extension method to validate options using FluentValidation
/// </summary>
public static class OptionsBuilderFluentValidationExtensions
{
	/// <summary>
	/// Adds FluentValidation based validation to the options builder
	/// </summary>
	/// <typeparam name="TOptions">The type of the options to validate</typeparam>
	/// <param name="optionsBuilder">The options builder to extend</param>
	/// <returns>The options builder, with validation added</returns>
	public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(
		this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
	{
		optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
			provider => new FluentValidationOptions<TOptions>(
				 provider, optionsBuilder.Name));

		return optionsBuilder;
	}
}
