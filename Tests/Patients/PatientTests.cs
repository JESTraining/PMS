using Domain.Entities;
using Domain.Exceptions;
using Xunit;


namespace Tests.Patients
{
    public class PatientTests
    {
        [Fact]
        public void Should_not_allow_future_birth_date()
        {
            Assert.Throws<DomainException>(() =>
                new Patient(
                    "Ana",
                    "López",
                    DateTime.Today.AddDays(1),
                    "9991234567",
                    "MRN-001"
                )
            );
        }

        [Fact]
        public void Should_create_valid_patient()
        {
            var patient = new Patient(
                "Carlos",
                "Gómez",
                new DateTime(1990, 5, 20),
                "9995554433",
                "MRN-003",
                "carlos@email.com"
            );

            Assert.NotNull(patient);
        }
    }
}
