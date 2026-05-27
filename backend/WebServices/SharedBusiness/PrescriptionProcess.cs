using Microsoft.AspNetCore.Mvc;

namespace WebServices.SharedBusiness
{
    public class PrescriptionProcess 
    {
        public record MedicationDto(
            int Id,
            string Name
        );

        public record PrescriptionMedicationDto(
            int Id,
            string Dosage,
            int Refills,
            MedicationDto Medication
        );

        public record PrescriptionDto(
            int Id,
            DateOnly IssueDate,
            IEnumerable<PrescriptionMedicationDto> Medications
        );

        public record UpdatePrescriptionDto(
            DateOnly IssueDate,
            IEnumerable<UpdatePrescriptionMedicationDto> Medications
        );

        public record UpdatePrescriptionMedicationDto(
            int MedicationId,
            string Dosage,
            int Refills
        );
    }
}
