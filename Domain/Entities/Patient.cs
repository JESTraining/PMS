using Domain.Exceptions;

namespace Domain.Entities
{
    public class Patient
    {
        public int Id { get; private set; }

        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public DateTime DateOfBirth { get; private set; }

        public string Phone { get; private set; } = null!;
        public string? Email { get; private set; }
        public string MedicalRecordNumber { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private Patient() { }

        public Patient(
            string firstName,
            string lastName,
            DateTime dateOfBirth,
            string phone,
            string medicalRecordNumber,
            string? email = null)
        {
            if (dateOfBirth > DateTime.Today)
                throw new DomainException("Date of birth cannot be in the future");

            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("First name is required");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Last name is required");

            if (string.IsNullOrWhiteSpace(medicalRecordNumber))
                throw new DomainException("Medical Record Number is required");

            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Phone = phone;
            MedicalRecordNumber = medicalRecordNumber;
            Email = email;
        }

        public bool IsMinor()
        {
            return DateOfBirth.AddYears(18) > DateTime.Today;
        }
    }

}
