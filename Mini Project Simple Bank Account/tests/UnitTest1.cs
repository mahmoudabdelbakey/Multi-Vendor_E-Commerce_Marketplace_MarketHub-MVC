using BankVault.Models;
using BankVault.Transactions;

namespace BankVault.Tests;

public class UnitTest1
{
    private static BankAccount CreateAccount(decimal balance = 100)
    {
        return new BankAccount("1001", "Ahmed", balance, AccountType.Savings);
    }

    // ===== Create =====

    [Fact]
    public void Create_ValidData_SetsProperties()
    {
        var account = CreateAccount(100);

        Assert.Equal("1001", account.AccountNumber);
        Assert.Equal("Ahmed", account.OwnerName);
        Assert.Equal(100, account.Balance);
    }

    [Fact]
    public void Create_NegativeBalance_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new BankAccount("1001", "Ahmed", -5, AccountType.Savings));
    }

    // ===== Deposit =====

    [Fact]
    public void Deposit_ValidAmount_IncreasesBalance()
    {
        var account = CreateAccount(100);

        account.Deposit(50);

        Assert.Equal(150, account.Balance);
    }

    [Fact]
    public void Deposit_NegativeAmount_Throws()
    {
        var account = CreateAccount(100);

        Assert.Throws<ArgumentException>(() => account.Deposit(-10));
    }

    [Fact]
    public void Deposit_Zero_Throws()
    {
        var account = CreateAccount(100);

        Assert.Throws<ArgumentException>(() => account.Deposit(0));
    }

    // ===== Withdraw =====

    [Fact]
    public void Withdraw_ValidAmount_DecreasesBalance()
    {
        var account = CreateAccount(100);

        account.Withdraw(40);

        Assert.Equal(60, account.Balance);
    }

    [Fact]
    public void Withdraw_MoreThanBalance_Throws()
    {
        var account = CreateAccount(100);

        Assert.Throws<InvalidOperationException>(() => account.Withdraw(500));
    }

    [Fact]
    public void Withdraw_NegativeAmount_Throws()
    {
        var account = CreateAccount(100);

        Assert.Throws<ArgumentException>(() => account.Withdraw(-10));
    }

    // ===== Transfer =====

    [Fact]
    public void Transfer_ValidAmount_MovesMoney()
    {
        var from = CreateAccount(100);
        var to = new BankAccount("1002", "Sara", 50, AccountType.Current);
        var service = new TransactionService(new TransactionHistory());

        service.Transfer(from, to, 30);

        Assert.Equal(70, from.Balance);
        Assert.Equal(80, to.Balance);
    }

    [Fact]
    public void Transfer_MoreThanBalance_Throws()
    {
        var from = CreateAccount(100);
        var to = new BankAccount("1002", "Sara", 50, AccountType.Current);
        var service = new TransactionService(new TransactionHistory());

        // ThrowsAny because the custom exception class is internal in the src project
        Assert.ThrowsAny<Exception>(() => service.Transfer(from, to, 500));

        Assert.Equal(100, from.Balance);
        Assert.Equal(50, to.Balance);
    }
}
