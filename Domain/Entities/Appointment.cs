namespace Domain.Entities
{
    public class Appointment
    {
        public int Id { get; private set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        public Doctor Doctor { get; set; } = default!;

        public Patient Patient { get; set; } = default!;
    }
}