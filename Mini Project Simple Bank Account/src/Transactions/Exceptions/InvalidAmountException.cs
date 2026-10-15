using System;
using System.Collections.Generic;
using System.Text;

namespace BankVault.Transactions.Exceptions
{
    internal class InvalidAmountException : Exception
    {
        public InvalidAmountException (string message) : base (message)
        {

        }
    }
}
