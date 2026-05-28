using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using WebServices.DataAccess;

namespace WebServices.Repositories
{
    public class BillingRepository
    {
        private readonly DatabaseContext _databaseContext;

        public BillingRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<UpsertRequest> CreateInvoice(InvoiceRequest invoice, Encounter encounter)
        {
            try
            {
                var totalAmount = invoice.InvoiceDetails
                    .Sum(d => (d.Quantity * d.UnitPrice));

                await _databaseContext.DBInvoices
                    .AddAsync(new Invoice
                    {
                        Patient = encounter.Patient,
                        Encounter = encounter,
                        Amount = totalAmount,
                        PaidAmount = totalAmount,
                        Status = InvoiceStatus.Paid,
                        IssuedDate = encounter.StartTime,
                        DueDate = DateTime.UtcNow,
                        PaidDate = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        InvoiceDetails = invoice.InvoiceDetails
                            .Select(d => new InvoiceDetail
                            {
                                Code = d.Code,
                                UnitPrice = d.UnitPrice,
                                Quantity = d.Quantity,
                                Price = d.Price,
                                Description = d.Description,
                            }).ToList(),
                    });

                encounter.Status = EncounterStatus.Completed;

                _databaseContext.SaveChanges();

                return new UpsertRequest { UpsertSuccessfull = true, Message = "Invoice created successfully." };
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new UpsertRequest { UpsertSuccessfull = false, Message = errorMsg };
            }
        }
    }
}
