namespace Domain.Entities
{
    public class Doctor
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<Appointment>? Appointments { get; set; }

        public ICollection<Prescriptions>? Prescriptions { get; set; }
    }
}