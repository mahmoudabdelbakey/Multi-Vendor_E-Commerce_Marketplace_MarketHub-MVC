using System;
using System.Collections.Generic;
using System.Text;

namespace BankVault.Transactions
{
   public class TransactionHistory
    {
        public List<string> Records { get; } = new();
        public void Add(string type,decimal amount)
        {
            Records.Add($"{DateTime.Now} | {type} | {amount : c}");
        }
    }
}
