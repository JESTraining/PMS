using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public enum InvoiceStatus
    {
        Pending = 1,
        Paid = 2,
        Overdue = 3,
        Cancelled = 4
    }
}
