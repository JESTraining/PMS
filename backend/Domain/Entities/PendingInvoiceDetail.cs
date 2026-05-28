namespace Domain.Entities
{
    /// <summary>
    /// PendingInvoiceDetail represents the details of a pending invoice, including its code and description. This class is used to encapsulate information about individual invoices that are yet to be settled.
    /// </summary>
    public class PendingInvoiceDetail
    {
        /// <summary>
        /// Gets or set the code for the detail as part of the invoice. This code is typically used to identify the specific item or service associated with the invoice.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item or service included in the invoice. This value indicates how many units of the item or service are being billed.
        /// </summary>
        public int Quantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the description for the detail as part of the invoice. This description provides additional context or information about the item or service being billed.
        /// </summary>
        public string Description { get; set; }
    }
}
