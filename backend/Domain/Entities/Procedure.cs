using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    /// <summary>
    /// Represents an action that is or was performed on a patient. This can be a physical intervention like an operation or a service like an ear cleaning.
    /// </summary>
    public class Procedure
    {
        /// <summary>Unique identifier for the procedure record.</summary>
        public int Id { get; set; }

        /// <summary>The encounter context in which this procedure was performed.</summary>
        public int EncounterId { get; set; }
        public Encounter Encounter { get; set; } = null!;

        /// <summary>The identifier of the patient receiving the procedure.</summary>
        public int PatientId { get; set; }

        /// <summary>A standardized terminology code for the procedure (e.g., CPT codes).</summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>A human-readable description of the procedure (e.g., "Ear Cleaning", "Suture Removal").</summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>The current state of the procedure.</summary>
        public ProcedureStatus Status { get; set; } = ProcedureStatus.InProgress;

        /// <summary>The date and time when the procedure was executed.</summary>
        public DateTime PerformedDate { get; set; } = DateTime.UtcNow;
    }
}