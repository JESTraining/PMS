using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Invoice
    {
        public int Id { get; private set; }

        public int PatientId { get; private set; }
        public Patient Patient { get; private set; } = null!;

        public decimal Amount { get; private set; }
        public decimal PaidAmount { get; private set; }

        public InvoiceStatus Status { get; private set; }

        public DateTime IssuedDate { get; private set; }
        public DateTime DueDate { get; private set; }
        public DateTime? PaidDate { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private Invoice() { }

        public Invoice(int patientId, decimal amount, DateTime dueDate)
        {
            if (amount <= 0)
                throw new DomainException("Invoice amount must be greater than zero.");

            if (dueDate <= DateTime.Today)
                throw new DomainException("Due date must be in the future.");

            PatientId = patientId;
            Amount = amount;
            PaidAmount = 0;
            Status = InvoiceStatus.Pending;
            IssuedDate = DateTime.UtcNow;
            DueDate = dueDate;
        }

        public void RegisterPayment(decimal paymentAmount)
        {
            if (paymentAmount <= 0)
                throw new DomainException("Payment amount must be greater than zero.");

            if (Status == InvoiceStatus.Paid)
                throw new DomainException("Invoice is already fully paid.");

            if (PaidAmount + paymentAmount > Amount)
                throw new DomainException("Payment exceeds invoice total.");

            PaidAmount += paymentAmount;

            if (PaidAmount == Amount)
            {
                Status = InvoiceStatus.Paid;
                PaidDate = DateTime.UtcNow;
            }
        }

        public void MarkAsOverdue()
        {
            if (Status == InvoiceStatus.Paid)
                throw new DomainException("Paid invoice cannot be marked as overdue.");

            Status = InvoiceStatus.Overdue;
        }

        public void Cancel()
        {
            if (PaidAmount > 0)
                throw new DomainException("Cannot cancel an invoice with payments.");

            Status = InvoiceStatus.Cancelled;
        }

    }
}
