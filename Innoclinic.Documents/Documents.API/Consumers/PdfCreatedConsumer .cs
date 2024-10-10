using Documents.BusinessLogic.Services.Interfaces;
using Innoclinic.Shared.MessageBrokers.Messages;
using MassTransit;

namespace Documents.API.Consumers;

/// <summary>
/// Consumer class for handling PDF creation events based on appointment results
/// </summary>
public class PdfCreatedConsumer : IConsumer<AppointmentResultCreatedMessage>
{
	private readonly IDocumentService _documentService;
	private readonly IPdfGenerator _pdfGenerator;
	private readonly ILogger<PdfCreatedConsumer> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="PdfCreatedConsumer"/> class
	/// </summary>
	/// <param name="documentService">The service used for document operations</param>
	/// <param name="logger">The logger instance for logging operations</param>
	/// <param name="pdfGenerator">The service used for generating PDF files</param>
	public PdfCreatedConsumer(IDocumentService documentService,
		ILogger<PdfCreatedConsumer> logger,
		IPdfGenerator pdfGenerator)
	{
		_documentService = documentService;
		_logger = logger;
		_pdfGenerator = pdfGenerator;
	}

	/// <summary>
	/// Consumes an appointment result message and generates a PDF document
	/// </summary>
	/// <param name="context">The context of the consumed message containing the appointment result data</param>
	public async Task Consume(ConsumeContext<AppointmentResultCreatedMessage> context)
	{
		_logger.LogInformation("Received message for ResultId: {ResultId}",
		context.Message.ResultId);

		var message = context.Message;

		var documentUrl = $"{message.ResultId}.pdf";

		_logger.LogInformation("Starting PDF upload for Result id: {ResultId}", message.ResultId);

		var pdf = _pdfGenerator.GeneratePdfFile(message);

		await _documentService.UploadPdfAsync(documentUrl, pdf, context.CancellationToken);

		_logger.LogInformation("PDF uploaded successfully for Result id: {ResultId}", context.Message.ResultId);
	}
}