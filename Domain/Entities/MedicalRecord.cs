namespace Domain.Entities
{
    /// <summary>
    /// Medical Record Entity Definition
    /// </summary>
    public class MedicalRecord
    {
        public int Id { get; private set; }

        public DateOnly VisitDate { get; set; }

        public string Vitals { get; set; } = string.Empty;

        public string DiagnosisCode { get; set; } = string.Empty;

        public string TreatmentPlan { get; set; } = string.Empty;

        public string ClinicalNotes { get; set; } = string.Empty;

        public Patient Patient { get; set; }
    }
}