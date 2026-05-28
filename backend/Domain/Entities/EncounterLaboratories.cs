namespace Domain.Entities
{
    /// <summary>
    /// Represents a collection of laboratory tests and related information associated with a specific encounter.
    /// </summary>
    /// <remarks>This class encapsulates the status, order date, and detailed results of laboratory tests for
    /// an encounter. It is typically used to track and manage laboratory workflows within the context of a patient
    /// visit or clinical event.</remarks>
    public class EncounterLaboratories
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date the order was placed.
        /// </summary>
        public DateOnly DateOrdered { get; set; } = default!;

        /// <summary>
        /// Gets or sets the current status of the laboratory.
        /// </summary>
        public LaboratoryStatus LaboratoryStatus { get; set; }

        /// <summary>
        /// Gets or sets the collection of laboratory details associated with the encounter.
        /// </summary>
        public List<EncounterLaboratoriesDetail>? LaboratoriesDetails { get; set; }

        /// <summary>
        /// Gets or sets the encounter associated with this instance.
        /// </summary>
        public Encounter? Encounter { get; set; }
    }
}
