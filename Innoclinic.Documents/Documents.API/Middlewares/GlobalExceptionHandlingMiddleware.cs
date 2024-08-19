using Innoclinic.Shared.ErrorModel;
using Innoclinic.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net.Mime;

namespace Documents.API.Middlewares;

/// <summary>
/// Represents a global exception middleware
/// </summary>
public static class GlobalExceptionHandlingMiddleware
{
	/// <summary>
	/// Configures exception handling middleware
	/// </summary>
	/// <param name="app">The WebApplication instance</param>
	public static void UseExceptionMiddleware(this WebApplication app)
	{
		app.UseExceptionHandler(appError =>
		{
			appError.Run(async context =>
			{
				context.Response.ContentType = MediaTypeNames.Application.Json;
				var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

				var (statusCode, message) = contextFeature?.Error switch
				{
					NotFoundException => (StatusCodes.Status404NotFound, contextFeature.Error.Message),
					_ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
				};

				context.Response.StatusCode = statusCode;
				await context.Response.WriteAsJsonAsync(new ErrorDetails
				{
					StatusCode = statusCode,
					Message = message
				});
			});
		});
	}
}
