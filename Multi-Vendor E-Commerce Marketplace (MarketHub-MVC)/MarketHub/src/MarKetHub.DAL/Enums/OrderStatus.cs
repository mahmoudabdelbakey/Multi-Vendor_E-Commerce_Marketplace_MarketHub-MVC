using System;
using System.Collections.Generic;
using System.Text;

namespace MarKetHub.DAL.Enums
{
    public enum OrderStatus
    {
        PendingPayment = 0,
        Paid = 1,
        Completed = 2,
        Cancelled = 3
    }
}
