namespace Domain.Entities
{
    /// <summary>
    /// Structure to Receive a create new invoice request
    /// </summary>
    public class InvoiceRequest
    {
        /// <summary>
        /// Gets or sets the Encounter ID values
        /// </summary>
        public int EncounterId { get; set; }

        /// <summary>
        /// Gets or set the list of items to be billed within the invoice
        /// </summary>
        public List<InvoiceDetailRequest> InvoiceDetails { get; set; }
    }
}
