using System;
using BankVault.Data;
using BankVault.Models;

namespace BankVault
{
    class Program
    {
        static void Main(string[] args)
        {
            FileAccountRepository repository = new FileAccountRepository();

            Console.WriteLine("Bank Account System");
            Console.WriteLine("===================");

            bool running = true;

            while (running)
            {
                Console.WriteLine();
                Console.WriteLine("1. Create Account");
                Console.WriteLine("2. View All Accounts");
                Console.WriteLine("3. Find Account");
                Console.WriteLine("4. Delete Account");
                Console.WriteLine("5. Exit");
                Console.Write("Choose option: ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Enter account number: ");
                    string accNum = Console.ReadLine();

                    Console.Write("Enter owner name: ");
                    string owner = Console.ReadLine();

                    Console.Write("Enter initial balance: ");
                    decimal balance = decimal.Parse(Console.ReadLine());

                    Console.Write("Enter account type (1=Savings, 2=Current): ");
                    string typeChoice = Console.ReadLine();
                    AccountType type;
                    if (typeChoice == "1")
                    {
                        type = AccountType.Savings;
                    }
                    else
                    {
                        type = AccountType.Current;
                    }

                    BankAccount account = new BankAccount(accNum, owner, balance, type);
                    repository.Add(account);
                    Console.WriteLine("Account created!");
                }
                else if (choice == "2")
                {
                    var accounts = repository.GetAll();
                    if (accounts.Count == 0)
                    {
                        Console.WriteLine("No accounts found.");
                    }
                    else
                    {
                        for (int i = 0; i < accounts.Count; i++)
                        {
                            Console.WriteLine(accounts[i].ToString());
                        }
                    }
                }
                else if (choice == "3")
                {
                    Console.Write("Enter account number: ");
                    string accNum = Console.ReadLine();

                    BankAccount account = repository.GetByAccountNumber(accNum);
                    if (account == null)
                    {
                        Console.WriteLine("Account not found.");
                    }
                    else
                    {
                        Console.WriteLine(account.ToString());
                    }
                }
                else if (choice == "4")
                {
                    Console.Write("Enter account number to delete: ");
                    string accNum = Console.ReadLine();

                    repository.Delete(accNum);
                    Console.WriteLine("Account deleted.");
                }
                else if (choice == "5")
                {
                    running = false;
                    Console.WriteLine("Goodbye!");
                }
                else
                {
                    Console.WriteLine("Invalid option.");
                }
            }
        }
    }
}
