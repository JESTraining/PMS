using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    /// <summary>
    /// Represents an interaction between a patient and healthcare provider(s) for the purpose of providing healthcare services or assessing the health status of a patient.
    /// Acts as the primary container for all clinical records generated during the visit.
    /// </summary>
    public class Encounter
    {
        /// <summary>Unique identifier for the clinical encounter.</summary>
        public int Id { get; set; }

        /// <summary>The identifier of the patient participating in the encounter.</summary>
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        /// <summary>The identifier of the primary doctor responsible for the encounter.</summary>
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        /// <summary>The identifier of the scheduled appointment that originated this encounter. May be null for walk-in patients.</summary>
        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        /// <summary>The current workflow status of the encounter.</summary>
        public EncounterStatus Status { get; set; } = EncounterStatus.Planned;

        /// <summary>The justification or explanation for the current status, particularly useful when an encounter is Cancelled or EnteredInError.</summary>
        public string? StatusReason { get; set; }

        /// <summary>The username or ID of the PMS user who last updated the status.</summary>
        public string? UpdatedBy { get; set; }

        /// <summary>The date and time the encounter actually began.</summary>
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        /// <summary>The date and time the encounter was officially concluded.</summary>
        public DateTime? EndTime { get; set; }

        // --- Clinical Navigation Properties ---

        /// <summary>Vital signs and other objective measurements taken during the encounter.</summary>
        public ICollection<ClinicalObservation>? Observations { get; set; }

        /// <summary>Diagnoses or clinical issues identified during the encounter.</summary>
        public ICollection<Condition>? Conditions { get; set; }

        /// <summary>Medical or physical interventions performed during the encounter.</summary>
        public ICollection<Procedure>? Procedures { get; set; }

        /// <summary>Allergies or adverse reactions discovered during the encounter.</summary>
        public ICollection<AllergyIntolerance>? Allergies { get; set; }

        /// <summary>Medication prescriptions issued during the encounter.</summary>
        public ICollection<Prescriptions>? Prescriptions { get; set; }

        /// <summary>The narrative clinical notes (e.g., SOAP notes) documented by the healthcare provider.</summary>
        public ClinicalNote? ClinicalNote { get; set; }

        /// <summary>Laboratory tests ordered during the encounter, including their results and interpretations.</summary>
        public ICollection<EncounterLaboratories>? Laboratories { get; set; }
    }
}