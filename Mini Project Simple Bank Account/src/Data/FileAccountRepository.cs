using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BankVault.Models;

namespace BankVault.Data
{
    public class FileAccountRepository : IAccountRepository
    {
        private List<BankAccount> accounts;
        private string filePath;

        public FileAccountRepository()
        {
            accounts = new List<BankAccount>();
            filePath = "data/accounts.json";

            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            LoadData();
        }

        public void Add(BankAccount account)
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                if (accounts[i].AccountNumber == account.AccountNumber)
                {
                    throw new Exception("Account already exists");
                }
            }
            accounts.Add(account);
            SaveChanges();
        }

        public BankAccount GetByAccountNumber(string accountNumber)
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                if (accounts[i].AccountNumber == accountNumber)
                {
                    return accounts[i];
                }
            }
            return null;
        }

        public List<BankAccount> GetAll()
        {
            return accounts;
        }

        public void Update(BankAccount account)
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                if (accounts[i].AccountNumber == account.AccountNumber)
                {
                    accounts[i] = account;
                    SaveChanges();
                    return;
                }
            }
        }

        public void Delete(string accountNumber)
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                if (accounts[i].AccountNumber == accountNumber)
                {
                    accounts.RemoveAt(i);
                    SaveChanges();
                    return;
                }
            }
        }

        public void SaveChanges()
        {
            List<AccountData> dataList = new List<AccountData>();

            for (int i = 0; i < accounts.Count; i++)
            {
                AccountData data = new AccountData();
                data.AccountNumber = accounts[i].AccountNumber;
                data.OwnerName = accounts[i].OwnerName;
                data.Balance = accounts[i].Balance;
                data.Type = accounts[i].Type.ToString();
                data.OpenedDate = accounts[i].OpenedDate;
                dataList.Add(data);
            }

            string json = JsonSerializer.Serialize(dataList);
            File.WriteAllText(filePath, json);
        }

        public void LoadData()
        {
            if (!File.Exists(filePath))
            {
                accounts = new List<BankAccount>();
                return;
            }

            string json = File.ReadAllText(filePath);

            if (string.IsNullOrEmpty(json))
            {
                accounts = new List<BankAccount>();
                return;
            }

            List<AccountData> dataList = JsonSerializer.Deserialize<List<AccountData>>(json);

            accounts = new List<BankAccount>();

            for (int i = 0; i < dataList.Count; i++)
            {
                AccountType type;
                if (dataList[i].Type == "Savings")
                {
                    type = AccountType.Savings;
                }
                else
                {
                    type = AccountType.Current;
                }

                BankAccount account = new BankAccount(
                    dataList[i].AccountNumber,
                    dataList[i].OwnerName,
                    dataList[i].Balance,
                    type
                );

                accounts.Add(account);
            }
        }
    }

    public class AccountData
    {
        public string AccountNumber { get; set; }
        public string OwnerName { get; set; }
        public decimal Balance { get; set; }
        public string Type { get; set; }
        public DateTime OpenedDate { get; set; }
    }
}
