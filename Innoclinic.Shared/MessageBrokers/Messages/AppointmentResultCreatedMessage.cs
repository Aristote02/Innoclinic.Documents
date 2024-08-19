namespace Innoclinic.Shared.MessageBrokers.Messages;

public class AppointmentResultCreatedMessage
{
	public Guid ResultId { get; set; }
	public DateTime Date { get; set; }
	public required string ServiceName { get; set; }
	public required string SpecializationName { get; init; }
	public required string PatientFullName { get; init; }
	public DateTime PatientBirthDate { get; set; }
	public required string DoctorFullName { get; init; }
	public required string Complaints { get; init; }
	public required string Conclusion { get; init; }
	public required string Recommendations { get; init; }
	public string PatientEmail { get; init; }
}