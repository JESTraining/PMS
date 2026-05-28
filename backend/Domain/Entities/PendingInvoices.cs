namespace Domain.Entities
{
    /// <summary>
    /// Provides a summary of pending invoices for a patient, including the patient's name, the date of the encounter, and a list of invoice details.
    /// </summary>
    public class PendingInvoices
    {
        /// <summary>
        /// Gets or sets Encounter ID for pending invoice
        /// </summary>
        public int EncounterId { get; set; }

        /// <summary>
        /// Patient's full name associated with the pending invoices.
        /// </summary>
        public string PatientName { get; set; } = string.Empty;

        /// <summary>
        /// Encounter date associated with the pending invoices. This represents the date of the medical encounter for which the invoices are pending.
        /// </summary>
        public DateOnly EncounterDate { get; set; }

        /// <summary>
        /// List of invoice details associated with the pending invoices. Each item in the list provides specific information about an individual invoice, such as the amount due, status, and other relevant details.
        /// </summary>
        public List<PendingInvoiceDetail> InvoiceDetails { get; set; } = new List<PendingInvoiceDetail>();
    }
}
