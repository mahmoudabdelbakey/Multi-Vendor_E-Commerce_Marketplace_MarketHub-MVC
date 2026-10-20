using System;
using System.Collections.Generic;
using BankVault.Models;

namespace BankVault.Data
{
    public interface IAccountRepository
    {
        void Add(BankAccount account);
        BankAccount GetByAccountNumber(string accountNumber);
        List<BankAccount> GetAll();
        void Update(BankAccount account);
        void Delete(string accountNumber);
        void SaveChanges();
        void LoadData();
    }
}
