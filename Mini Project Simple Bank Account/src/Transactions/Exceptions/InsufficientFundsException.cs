using System;
using System.Collections.Generic;
using System.Text;

namespace BankVault.Transactions.Exceptions
{
    internal class InsufficientFundsException : Exception
    {
        public InsufficientFundsException (string message) : base(message) { }
    }
}
