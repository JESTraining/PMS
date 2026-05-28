namespace Domain.Entities
{
    /// <summary>
    /// Represents a laboratory service, including its description, pricing, preparation requirements, and associated
    /// encounter information.
    /// </summary>
    public class Laboratory
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the description associated with all that involves the Laboratory.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price of the laboratory service.
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Gets or sets the estimated time required to complete the operation, in hours.
        /// </summary>
        public int TimeToCompleteInHours { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether food is prohibited before execution.
        /// </summary>
        public bool NoFoodBeforeExecuted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether liquid ingestion should occur before execution.
        /// </summary>
        public bool LiquidIngestionBeforeExecuted { get; set; }

        /// <summary>
        /// Gets or sets the detail information for laboratory results associated with the encounter.
        /// </summary>
        public ICollection<EncounterLaboratoriesDetail>? EncounterLaboratoriesDetail { get; set; }
    }
}
