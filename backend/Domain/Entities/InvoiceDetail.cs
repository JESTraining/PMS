namespace Domain.Entities
{
    /// <summary>
    /// Represents the details of an individual item or service included in a financial invoice. It contains information such as the code, unit price, quantity, total price, and description of the item or service. This class is used to encapsulate the specific components that make up an invoice, allowing for detailed billing and record-keeping.
    /// </summary>
    public class InvoiceDetail
    {
        /// <summary>
        /// Gets or sets the unique identifier for the invoice detail.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code for the detail as part of the invoice. This code is typically used to identify the specific item or service associated with the invoice.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unit price for the item or service included in the invoice. This value represents the cost per single unit of the item or service.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item or service included in the invoice. This value indicates how many units of the item or service are being billed.
        /// </summary>
        public int Quantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total price for the item or service included in the invoice. This value is typically calculated as the product of the unit price and quantity, representing the total cost for that specific line item.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the description for the detail as part of the invoice. This description provides additional context or information about the item or service being billed.
        /// </summary>
        public string Description { get; set; } = string.Empty;
        public Invoice Invoice { get; set; } = default!;
    }
}
