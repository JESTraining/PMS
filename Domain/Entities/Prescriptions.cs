namespace Domain.Entities
{
    public class Prescriptions
    {
        public int Id { get; set; }

        public string Medication { get; set; } = string.Empty;

        public string Dosage { get; set; } = string.Empty;

        public int Refils { get; set; }

        public DateOnly IssueDate { get; set; }

        public Doctor Doctor { get; set; } = default!;

        public Patient Patient { get; set; } = default!;
    }
}