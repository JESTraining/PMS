using System;
using System.Collections.Generic;
using System.Text;

using Domain.Entities;
using Domain.Exceptions;
using Xunit;


namespace Tests.Invoices
{
    public class InvoiceTests
    {
        [Fact]
        public void Should_Create_invoice_with_valid_data()
        {
            var invoice = new Invoice(
                patientId: 1,
                amount: 1500m,
                dueDate: DateTime.Today.AddDays(30)
                );

            Assert.NotNull(invoice);
            Assert.Equal(1500m, invoice.Amount);
            Assert.Equal(0m, invoice.PaidAmount);
            Assert.Equal(InvoiceStatus.Pending, invoice.Status);
        }

        [Fact]
        public void Should_not_allow_invoice_with_zero_or_negative_amount()
        {
            Assert.Throws<DomainException>(() =>
                new Invoice(1, 0, DateTime.Today.AddDays(10))
            );
        }

        [Fact]
        public void Should_register_partial_payment()
        {
            var invoice = new Invoice(1, 1000m, DateTime.Today.AddDays(10));

            invoice.RegisterPayment(400m);

            Assert.Equal(400m, invoice.PaidAmount);
            Assert.Equal(InvoiceStatus.Pending, invoice.Status);
        }

        [Fact]
        public void Should_mark_invoice_as_paid_when_fully_paid()
        {
            var invoice = new Invoice(1, 1000m, DateTime.Today.AddDays(10));

            invoice.RegisterPayment(1000m);

            Assert.Equal(InvoiceStatus.Paid, invoice.Status);
            Assert.NotNull(invoice.PaidDate);
        }
        [Fact]
        public void Should_not_allow_payment_exceeding_invoice_amount()
        {
            var invoice = new Invoice(1, 1000m, DateTime.Today.AddDays(10));

            Assert.Throws<DomainException>(() =>
                invoice.RegisterPayment(1200m)
            );
        }
        [Fact]
        public void Should_not_allow_paying_paid_invoice()
        {
            var invoice = new Invoice(1, 1000m, DateTime.Today.AddDays(10));

            invoice.RegisterPayment(1000m);

            Assert.Throws<DomainException>(() =>
                invoice.RegisterPayment(100m)
                );
        }
        [Fact]
        public void Should_mark_invoice_as_overduw()
        {
            var invoice = new Invoice(1, 500m, DateTime.Today.AddDays(10));

            invoice.MarkAsOverdue();

            Assert.Equal(InvoiceStatus.Overdue, invoice.Status);
        }
        [Fact]
        public void Should_not_cancel_invoice_with_payments()
        {
            var invoice = new Invoice(1, 800m, DateTime.Today.AddDays(10));

            invoice.RegisterPayment(200m);

            Assert.Throws<DomainException>(() =>
            invoice.Cancel()
            );
        }

    }
}
