# BankVault — Simple Bank Account Console App

A small C# / .NET console app for managing a bank account: deposit, withdraw, transfer, view balance and see the transaction history. Account data is saved to a local JSON file so it survives between runs.

Built as a team mini project (5 members) to practice **OOP fundamentals**: classes, encapsulation, constructors, custom exceptions, interfaces and separation of concerns.

## Requirements

- .NET 10 SDK
- Visual Studio 2022+ (or the `dotnet` CLI)

## How to run

```bash
# run the app
dotnet run --project src/BankVault.csproj

# run the unit tests
dotnet test tests/BankVault.Tests.csproj
```

> **Windows tip:** keep the project in a short path (e.g. `C:\BankVault`). Very long folder paths make the build fail with `MSB3030` / "Could not find a part of the path".

In Visual Studio: run the app with the green **Run** button (startup project = `BankVault`), and run the tests from **Test → Run All Tests** (Test Explorer).

## Project structure

```
BankVault/
├── BankVault.slnx
├── README.md
├── src/
│   ├── Models/          BankAccount, Customer              (Member 1)
│   ├── Transactions/    TransactionService, TransactionHistory,
│   │   └── Exceptions/  InsufficientFundsException, InvalidAmountException (Member 2)
│   ├── UI/              MainMenu, ConsoleHelper            (Member 3)
│   ├── Data/            IAccountRepository, FileAccountRepository,
│   │   └── data/        accounts.json                      (Member 4)
│   └── Program.cs       connects all the parts together
└── tests/               xUnit tests                        (Member 5)
    ├── BankVault.Tests.csproj
    └── UnitTest1.cs
```

## How it works

| Layer | What it does |
|---|---|
| **Models** | `BankAccount` holds the account number, owner, balance (`private set`) and validates deposits/withdrawals. `Customer` holds customer info. |
| **Transactions** | `TransactionService` runs Deposit / Withdraw / Transfer, validates the amount, throws custom exceptions and logs each operation in `TransactionHistory`. |
| **UI** | `MainMenu` shows a switch-based menu. `ConsoleHelper` reads numbers safely with `TryParse`. |
| **Data** | `FileAccountRepository` saves and loads accounts as JSON through the `IAccountRepository` interface. |
| **Program.cs** | Loads the accounts, creates a sample account if none exist, runs the menu, then saves on exit. |

## Menu

```
1. Deposit
2. Withdraw
3. Transfer
4. View Balance
5. Transaction History
0. Exit
```

## Tests

10 xUnit tests in `tests/UnitTest1.cs`, named `Method_Scenario_ExpectedResult`:

- **Create:** valid data sets the properties, negative initial balance throws.
- **Deposit:** valid amount increases balance, negative amount and zero throw.
- **Withdraw:** valid amount decreases balance, more than balance and negative amount throw.
- **Transfer:** valid transfer moves the money, transfer above the balance fails and leaves both balances unchanged.

## Known issues (found during review)

These are still open and should be fixed by the owner of each part:

1. **`TransactionService.Withdraw`** uses `amount < account.Balance` to detect insufficient funds. It should be `amount > account.Balance`. Right now a withdrawal is rejected when the balance is enough.
2. **Custom exceptions are `internal`** (`InsufficientFundsException`, `InvalidAmountException`). They should be `public` so the test project can use `Assert.Throws<...>` on them. Until then the transfer-failure test uses `Assert.ThrowsAny<Exception>`.
3. **`TransactionHistory.Add`** formats the amount as `{amount : c}` (extra space). It should be `{amount:C}`.
4. **Transfer in the menu** is not connected: `MainMenu` works on a single account, so option 3 prints "Not connected yet".
5. **`OpenedDate`** is reset to the current time every time accounts are loaded from the file.
6. The transaction history is kept in memory only and is lost when the app closes.
