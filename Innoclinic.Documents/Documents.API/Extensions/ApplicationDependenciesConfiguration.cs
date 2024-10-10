#region
using Azure.Storage.Blobs;
using Documents.API.Consumers;
using Documents.BusinessLogic.Configurations;
using Documents.BusinessLogic.Services.Implementations;
using Documents.BusinessLogic.Services.Interfaces;
using Documents.BusinessLogic.Validators;
using FluentValidation;
using Innoclinic.Shared.MessageBrokers.Configurations;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
#endregion

namespace Documents.API.Extensions;

public static class ApplicationDependenciesConfiguration
{
	/// <summary>
	/// Adds the services of the application
	/// </summary>
	/// <param name="services"></param>
	public static IServiceCollection AddServices(this IServiceCollection services)
	{
		return services.AddScoped<IDocumentService, DocumentService>()
			.AddScoped<IPdfGenerator, PdfGenerator>()
			.AddScoped<IBlobClientFactory, BlobClientFactory>();
	}

	/// <summary>
	/// Configures Cross-Origin Resource Sharing (CORS) for the application
	/// </summary>
	/// <param name="builder">The WebApplicationBuilder used to configure services and middleware</param>
	public static void ConfigureCrossOriginRessourceSharing(this WebApplicationBuilder builder)
	{
		builder.Services.AddCors(options =>
		{
			options.AddDefaultPolicy(
				policy =>
				{
					policy.WithOrigins("*")
					.AllowAnyHeader()
					.AllowAnyMethod();
				});
		});
	}

	/// <summary>
	/// Configures the Blob Storage services by setting up 
	/// the necessary configurations and services for dependency injection
	/// </summary>
	/// <param name="services">The <see cref="IServiceCollection"/> instance to add the services to</param>
	/// <param name="configuration">The <see cref="IConfiguration"/> instance used to retrieve configuration settings</param>
	/// <returns>Returns the modified <see cref="IServiceCollection"/> instance with the configured services</returns>
	/// <exception cref="InvalidOperationException">Thrown when the Blob Storage connection string is null or empty</exception>
	public static IServiceCollection ConfigureBlobStorageServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<BlobStorageConfigurations>(configuration.GetSection("BlobStorageConfigurations"));
		services.AddScoped<BlobServiceClient>(x =>
		{
			var options = x.GetRequiredService<IOptions<BlobStorageConfigurations>>().Value;
			if (string.IsNullOrEmpty(options.ConnectionString))
			{
				throw new ArgumentException("The connection string for the blob storage was not found, it can't be null or empty");
			}

			return new BlobServiceClient(options.ConnectionString);
		});

		return services;
	}

	/// <summary>
	/// Configures the JWT authentication
	/// </summary>
	/// <param name="builder">The WebApplicationBuilder</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Thrown if the operation fails</exception>
	public static IServiceCollection ConfigureJwtAuthentication(this WebApplicationBuilder builder)
	{
		var jwtSection = builder.Configuration.GetSection("Jwt");
		if (!jwtSection.Exists() || !ValidateJwtSettings(jwtSection))
		{
			throw new InvalidOperationException("JWT configuration values are missing or invalid");
		}

		builder.Services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSection["Issuer"],
					ValidAudience = jwtSection["Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!))
				};
			});

		return builder.Services;
	}

	/// <summary>
	/// Configure swagger for API documentation
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	public static IServiceCollection ConfigureSwaggerGen(this IServiceCollection services)
	{
		services.AddSwaggerGen(c =>
		{
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Description = "Standard Authorisation header using the scheme (\"bearer {token}\")",
				In = ParameterLocation.Header,
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey
			});

			c.OperationFilter<SecurityRequirementsOperationFilter>();

			c.SwaggerDoc("v1", new OpenApiInfo { Title = "Documents API", Version = "v1" });
			var xmlPath = Path.Combine(AppContext.BaseDirectory, "Documents.API.xml");
			c.IncludeXmlComments(xmlPath);
		});

		return services;
	}

	/// <summary>
	/// Configures MassTransit with RabbitMQ for message handling
	/// </summary>
	/// <param name="services">The service collection to add MassTransit services to</param>
	/// <param name="configuration">Configuration settings for RabbitMQ</param>
	/// <returns>The updated service collection with MassTransit configured</returns>
	public static IServiceCollection ConfigureMassTransit(this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddScoped<IValidator<RabbitMQConfigurations>, RabbitMQConfigurationsValidator>();

		services.AddOptions<RabbitMQConfigurations>()
			.Bind(configuration.GetSection("RabbitMQ"))
			.ValidateFluentValidation()
			.ValidateOnStart();

		services.AddMassTransit(x =>
		{
			x.AddConsumer<PdfCreatedConsumer>();

			x.UsingRabbitMq((context, cfg) =>
			{
				var options = context.GetRequiredService<IOptions<RabbitMQConfigurations>>().Value;

				cfg.Host(options.Host, h =>
				{
					h.Username(options.UserName);
					h.Password(options.Password);
				});

				cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(true));

				cfg.ReceiveEndpoint("pdf-upload-queue", e =>
				{
					e.ConfigureConsumer<PdfCreatedConsumer>(context);
				});
			});
		});

		return services;
	}

	/// <summary>
	/// Validates the JWT settings from the configurations
	/// </summary>
	/// <param name="jwtSection">The configuration section containing JWT settings</param>
	/// <returns>True if the settings are valid, otherwise false</returns>
	private static bool ValidateJwtSettings(IConfigurationSection jwtSection)
	{
		return !string.IsNullOrEmpty(jwtSection["Issuer"]) &&
			   !string.IsNullOrEmpty(jwtSection["Audience"]) &&
			   !string.IsNullOrEmpty(jwtSection["Key"]);
	}
}