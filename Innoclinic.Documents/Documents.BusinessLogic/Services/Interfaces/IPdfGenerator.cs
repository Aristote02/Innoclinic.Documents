namespace Documents.BusinessLogic.Services.Interfaces;

/// <summary>
/// Represents and interface for generating PDF files
/// </summary>
public interface IPdfGenerator
{
	/// <summary>
	/// Generates a PDF file based on appointment result data
	/// </summary>
	/// <param name="pdfUpload">The appointment result data used for generating the PDF</param>
	/// <returns>A byte array representing the generated PDF file</returns>
	byte[] GeneratePdfFile(AppointmentResultCreatedMessage pdfUpload);
}
