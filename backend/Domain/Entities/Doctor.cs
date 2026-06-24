namespace Domain.Entities
{
    /// <summary>
    /// Represents a medical doctor or provider who can be assigned to appointments
    /// and can author prescriptions for patients.
    /// </summary>
    public class Doctor
    {
        /// <summary>
        /// Unique identifier for the doctor.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The full name of the doctor.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Medical specialty of the doctor (e.g. "General Medicine", "Pediatrics").
        /// </summary>
        public string? Specialty { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        /// <summary>
        /// Collection of appointments associated with the doctor. May be null if
        /// appointments have not been loaded or the doctor has none.
        /// </summary>
        public ICollection<Appointment>? Appointments { get; set; }

        /// <summary>
        /// Collection of prescriptions authored by the doctor. May be null if
        /// prescriptions have not been loaded or the doctor has none.
        /// </summary>
        public ICollection<Prescriptions>? Prescriptions { get; set; }

        /// <summary>
        /// Collection of rest schedules associated with the doctor. May be null if
        /// rest schedules have not been loaded or the doctor has none.
        /// </summary>
        public ICollection<DoctorRestSchedule>? DoctorRestSchedules { get; set; }
    }
}