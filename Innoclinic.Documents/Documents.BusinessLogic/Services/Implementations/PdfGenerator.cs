using Documents.BusinessLogic.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace Documents.BusinessLogic.Services.Implementations;

/// <summary>
/// Represents a class that provides functionality for generating PDF files
/// </summary>
public class PdfGenerator : IPdfGenerator
{
	/// <summary>
	/// Generates a PDF file based on the details provided in the <see cref="AppointmentResultCreatedMessage"/>.
	/// </summary>
	/// <param name="pdfUpload">An object containing the details to be included in the PDF report</param>
	/// <returns>A byte array representing the generated PDF file</returns>
	public byte[] GeneratePdfFile(AppointmentResultCreatedMessage pdfUpload)
	{
		QuestPDF.Settings.License = LicenseType.Community;

		var pdfFile = Document.Create(container =>
		{
			container.Page(page =>
			{
				page.Size(PageSizes.A4);
				page.Margin(1.5f, Unit.Centimetre);
				page.PageColor(Colors.White);
				page.DefaultTextStyle(x => x.FontSize(16).FontFamily(Fonts.Arial));

				page.Header()
					.AlignCenter()
					.Text("Medical Appointment Report")
					.Bold()
					.FontSize(24)
					.FontColor(Colors.Black);

				page.Content()
					.PaddingVertical(1, Unit.Centimetre)
					.Column(x =>
					{
						x.Spacing(4);

						x.Item().Text("Appointment Date:").FontSize(18).SemiBold();
						x.Item().Text(pdfUpload.Date.ToString("dd MMMM yyyy, HH:mm")).FontColor(Colors.Blue.Darken4);

						x.Item().PaddingTop(10f).Text("Patient Name:").FontSize(18).SemiBold();
						x.Item().Text(pdfUpload.PatientFullName).FontColor(Colors.Blue.Darken4);

						x.Item().PaddingTop(10f).Text("Date of Birth:").FontSize(18).SemiBold();
						x.Item().Text(pdfUpload.PatientBirthDate.ToShortDateString()).FontColor(Colors.Blue.Darken4);

						x.Item().PaddingTop(10f).Text("Doctor's Name:").FontSize(18).SemiBold();
						x.Item().Text(pdfUpload.DoctorFullName).FontColor(Colors.Blue.Darken4);

						x.Item().PaddingTop(10f).Text("Specialization:").FontSize(18).SemiBold();
						x.Item().Text(pdfUpload.SpecializationName).FontColor(Colors.Blue.Darken4);

						x.Item().PaddingTop(10f).Text("Service Provided:").FontSize(18).SemiBold();
						x.Item().Text(pdfUpload.ServiceName).FontColor(Colors.Blue.Darken4);

						x.Item().PaddingTop(10f).Text("Patient Complaints:").FontSize(18).SemiBold();
						x.Item().Text(pdfUpload.Complaints).FontColor(Colors.Blue.Darken4);

						x.Item().PaddingTop(10f).Text("Conclusions:").FontSize(18).SemiBold();
						x.Item().Text(pdfUpload.Conclusion).FontColor(Colors.Blue.Darken4);

						x.Item().PaddingTop(10f).Text("Recommendations:").FontSize(18).SemiBold();
						x.Item().Text(pdfUpload.Recommendations).FontColor(Colors.Blue.Darken4);
					});

				page.Footer().Text("Thank you for choosing our clinic. We wish you continued good health and look forward to serving you again.").AlignCenter().FontSize(12);
			});
		}).GeneratePdf();

		return pdfFile;
	}
}
