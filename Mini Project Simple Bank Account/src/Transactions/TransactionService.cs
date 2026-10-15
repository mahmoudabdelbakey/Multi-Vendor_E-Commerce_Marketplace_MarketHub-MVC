using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using BankVault.Models;
using BankVault.Transactions.Exceptions;

namespace BankVault.Transactions
{
    public class TransactionService 
    {
        public TransactionHistory History { get; } = new();
        public TransactionService(TransactionHistory history) {  History = history; }

        public void Deposit(BankAccount account,decimal amount)
        {
            ValidateAmount(amount);

            account.Deposit(amount);

            History.Add("Deposit", amount);
        }
        public void Withdraw(BankAccount account, decimal amount)
        {

            ValidateAmount(amount);
            if(amount < account.Balance)
            {
                throw new InsufficientFundsException("You Can't Withdraw this amount !!");

            }
            account.Withdraw(amount);

            History.Add("Withdraw", amount);


        }
        public void Transfer(
       BankAccount _from,
       BankAccount _to,
       decimal amount)
        {
            ValidateAmount(amount);


            if (amount > _from.Balance)
            {
                throw new InsufficientFundsException(
                    "You don't have this amount of money to transfer");
            }

            _from.Withdraw(amount);
            _to.Deposit(amount);

            History.Add("Transfer", amount);

        }
        static void ValidateAmount(decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidAmountException(
                    "Amount must be greater than zero.");
            }

        }
        }
}
