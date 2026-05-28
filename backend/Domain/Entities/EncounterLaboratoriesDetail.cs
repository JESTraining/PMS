namespace Domain.Entities
{
    /// <summary>
    /// Represents the details of a laboratory entry associated with a specific encounter.
    /// </summary>
    public  class EncounterLaboratoriesDetail
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the laboratory results associated with the encounter.
        /// </summary>
        public EncounterLaboratories? EncounterLaboratories { get; set; }

        /// <summary>
        /// Gets or sets the laboratory associated with this entity.
        /// </summary>
        public Laboratory? Laboratory { get; set; }
    }
}
