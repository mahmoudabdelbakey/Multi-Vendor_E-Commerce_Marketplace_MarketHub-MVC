using System;
using System.Collections.Generic;
using System.Text;
using BankVault.Models;
using BankVault.Transactions;
using BankVault.Transactions.Exceptions;

namespace BankVault.UI
{
    internal class MainMenu
    {
        private readonly TransactionService transactionService;
        private readonly BankAccount account;

        public MainMenu(TransactionService service, BankAccount bankAccount)
        {
            transactionService = service;
            account = bankAccount;
        }

        public void Run()
        {
            int choice;

            do
            {
                Console.Clear();

                Console.WriteLine("==============================");
                Console.WriteLine("       BANK VAULT SYSTEM      ");
                Console.WriteLine("==============================");
                Console.WriteLine("1. Deposit");
                Console.WriteLine("2. Withdraw");
                Console.WriteLine("3. Transfer");
                Console.WriteLine("4. View Balance");
                Console.WriteLine("5. Transaction History");
                Console.WriteLine("0. Exit");
                Console.WriteLine("==============================");

                choice = ConsoleHelper.ReadInt("Enter your choice: ");

                try
                {
                    switch (choice)
                    {
                        case 1:
                            decimal depositAmount = ConsoleHelper.ReadAmount("Enter deposit amount: ");

                            transactionService.Deposit(account, depositAmount);
                            Console.WriteLine("Deposit completed successfully.");
                            break;

                        case 2:
                            decimal withdrawAmount = ConsoleHelper.ReadAmount("Enter withdrawal amount: ");

                            transactionService.Withdraw(account, withdrawAmount);
                            Console.WriteLine("Withdrawal completed successfully.");
                            break;

                        case 3:
                            Console.WriteLine(
                                "Transfer requires a second account. Not connected yet.");
                            break;

                        case 4:
                            Console.WriteLine($"Current balance: {account.Balance:C}");
                            break;

                        case 5:
                            foreach (string record in transactionService.History.Records)
                            {
                                Console.WriteLine(record);
                            }
                            break;

                        case 0:
                            Console.WriteLine("Thank you for using Bank Vault!");
                            break;

                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (InvalidAmountException ex)
                {
                    Console.WriteLine($"Invalid amount: {ex.Message}");
                }
                catch (InsufficientFundsException ex)
                {
                    Console.WriteLine($"Transaction failed: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Transaction failed: {ex.Message}");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Transaction failed: {ex.Message}");
                }

                if (choice != 0)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }

            } while (choice != 0);
        }
    }
}
