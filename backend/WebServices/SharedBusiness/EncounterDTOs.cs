namespace WebServices.SharedBusiness
{
    public record EncounterSummaryDto(
        int Id,
        int? AppointmentId,
        string Status,
        string PatientName, 
        string? Subjective,
        string? Objective,
        string? Assessment,
        string? Plan,
        int ObservationsCount,
        int ConditionsCount,
        int ProceduresCount,
        int AllergiesCount,
        int PrescriptionsCount,
        IEnumerable<object> Observations,
        IEnumerable<object> Allergies
    );
    public record EncounterDetailDto(
        int Id,
        string PatientName,

        // 🔥 ENCOUNTER
        DateTime StartTime,
        DateTime? EndTime,
        string Status,

        // 🔥 APPOINTMENT
        int AppointmentId,
        string Reason,
        DateTime AppointmentStart,
        DateTime AppointmentEnd,

        IEnumerable<object> ClinicalObservations,
        IEnumerable<object> Conditions,
        IEnumerable<object> ClinicalNotes,
        IEnumerable<object> Prescriptions
    );
    public record UpdateAppointmentDto(
        string Reason,
        DateTime StartTime,
        DateTime EndTime
    );
    public class CreatePrescriptionDto
    {
        public int MedicationId { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
    }
    public record UpdateClinicalNoteRequest(
        string? Subjective,
        string? Objective,
        string? Assessment,
        string? Plan
    );
    public record UpdateObservationDto(
        string? Category,
        string? DisplayName,
        string? ValueString,
        string? Unit
    );

    public record CreateObservationDto(string Category, string DisplayName, string ValueString, string Unit);
    public record CreateAllergyDto(string Substance, string Criticality, string Reaction);
    public record CreateConditionDto(string Code, string DisplayName, string ClinicalStatus);
}