using System;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a financial invoice billed to a patient.
    /// Acts as a data container (Anemic Domain Model) mapped to the database.
    /// </summary>
    public class Invoice
    {
        /// <summary>
        /// The unique identifier for the invoice.
        /// </summary>
        public int Id { get; set; }

        // Navigation Properties

        /// <summary>
        /// The patient associated with this invoice.
        /// </summary>
        public Patient Patient { get; set; } = null!;

        /// <summary>
        /// The total amount to be billed.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The amount that has been paid so far.
        /// </summary>
        public decimal PaidAmount { get; set; }

        /// <summary>
        /// The current status of the invoice (e.g., Pending, Paid, Overdue, Cancelled).
        /// </summary>
        public InvoiceStatus Status { get; set; }

        /// <summary>
        /// The date and time when the invoice was officially issued.
        /// </summary>
        public DateTime IssuedDate { get; set; }

        /// <summary>
        /// The deadline date for the payment.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// The date and time when the invoice was fully paid. Null if the invoice is not fully paid yet.
        /// </summary>
        public DateTime? PaidDate { get; set; }

        /// <summary>
        /// The timestamp indicating when the invoice record was created. Defaults to the current UTC time.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Encounter associated with this invoice, if applicable. This property is optional and may be null if the invoice is not linked to a specific encounter.
        /// </summary>
        public Encounter? Encounter { get; set; } = default!;

        /// <summary>
        /// Details of the invoice, including individual line items and their respective costs. This collection may be null if there are no details associated with the invoice.
        /// </summary>
        public IEnumerable<InvoiceDetail>? InvoiceDetails { get; set; } = default!;

        /// <summary>
        /// Parameterless constructor required by Entity Framework Core.
        /// </summary>
        public Invoice() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Invoice"/> class.
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient being billed.</param>
        /// <param name="amount">The total amount of the invoice.</param>
        /// <param name="dueDate">The deadline for the payment.</param>
        public Invoice(int patientId, decimal amount, DateTime dueDate)
        {
            // Note: The patientId is used by EF Core to establish the foreign key relationship 
            // if configured, even though the property itself is not explicitly defined in this model.
            Amount = amount;
            PaidAmount = 0;
            Status = InvoiceStatus.Pending;
            IssuedDate = DateTime.UtcNow;
            DueDate = dueDate;
        }
    }
}