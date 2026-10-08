# BankVault — Simple Bank Account Console App
Mini Project — C# / .NET Console (OOP Fundamentals)

---

## 🎯 Project overview

A console-based banking system that lets a user create accounts, deposit, withdraw, transfer money between accounts, and view transaction history — all data persisted to a local file so it survives between runs.

Built to demonstrate solid **OOP fundamentals**: classes, encapsulation, constructors, custom exceptions, and clean separation of concerns — expanded just enough from the original single-class idea so a team of 5 can each own a real, independent piece.

| | |
|---|---|
| **Team size** | 5 members |
| **Timeline** | 1–2 weeks |
| **Core stack** | C#, .NET Console App, OOP, (optional) System.Text.Json for persistence, xUnit for tests |
| **Based on** | ITC Mini Project #4 — Simple Bank Account (expanded for team-scale scope) |

---

## 🧩 Why split it this way?

Instead of 5 people touching one small class, the project is expanded into 5 independent layers — each with a clear file, a clear owner, and a clear "done" definition, so everyone can work in parallel without blocking each other.

```
BankVault/
├── BankVault.sln
├── README.md
├── src/
│   ├── Models/              → Member 1
│   │   ├── BankAccount.cs
│   │   └── Customer.cs
│   ├── Transactions/        → Member 2
│   │   ├── TransactionService.cs
│   │   ├── TransactionHistory.cs
│   │   └── Exceptions/
│   │       ├── InsufficientFundsException.cs
│   │       └── InvalidAmountException.cs
│   ├── UI/                  → Member 3
│   │   ├── MainMenu.cs
│   │   └── ConsoleHelper.cs
│   ├── Data/                → Member 4
│   │   ├── IAccountRepository.cs
│   │   ├── FileAccountRepository.cs
│   │   └── data/accounts.json
│   └── Program.cs
└── tests/                   → Member 5
    ├── BankVault.Tests.csproj
    └── BankAccountTests.cs
```

---

## 👥 Team & responsibilities

| Member | Role | Owns | Deliverable |
|---|---|---|---|
| **Member 1** | Core Domain / OOP Lead | `Models/BankAccount.cs`, `Models/Customer.cs` | `BankAccount` class with `AccountNumber`, `OwnerName`, `Balance` (`private set`), constructors, basic validation (no negative balance on creation) |
| **Member 2** | Transactions & Business Logic | `Transactions/` folder | `TransactionService` with `Deposit()`, `Withdraw()`, `Transfer()`; `TransactionHistory` log (timestamp + type + amount); custom exceptions for invalid amount / insufficient funds |
| **Member 3** | Console UI / Menu System | `UI/` folder | Main menu (switch-based), input validation (`int.TryParse`, `decimal.TryParse`), routes user choices to the right service calls, formatted console output |
| **Member 4** | Data Persistence | `Data/` folder | `IAccountRepository` interface + `FileAccountRepository` implementation — saves/loads accounts to a local JSON file so data isn't lost between runs |
| **Member 5** | Testing & Docs | `tests/` folder + root docs | Unit tests for deposit/withdraw/transfer edge cases (negative amount, insufficient funds), final `README` walkthrough, demo script for presentation |

> **Integration point:** `Program.cs` is the only file that touches all four modules (Models, Transactions, UI, Data) — whoever finishes first can draft it, then the team reviews it together before the final merge.

---

## ✅ Status — what's done vs. still pending

**Done:**
- Core idea finalized (Simple Bank Account, expanded to team scale)
- Folder structure & ownership agreed
- Roles assigned (5 members)

**Still pending:**
- All 5 modules above (not started yet)
- `Program.cs` integration
- Unit test suite
- Final demo walkthrough

---

## 🔀 Git workflow

1. **Branch naming:** `feature/<module-name>` — e.g. `feature/transactions`, `feature/ui-menu`, `feature/persistence`, `feature/tests`
2. **Each member:**
   - Works only inside their owned folder (see table above) to avoid merge conflicts
   - Commits small, focused changes with clear messages (`feat: add Deposit method with validation`)
   - Opens a Pull Request into `main` when their module is ready
3. **Before merging:** at least one other teammate reviews the PR
4. **Integration:** once all 4 modules (Models, Transactions, UI, Data) are merged, whoever is free wires them together in `Program.cs`, then Member 5 runs the full test pass

---

## 📋 Definition of Done (per module)

- [ ] Code compiles with no warnings
- [ ] No `Balance` or sensitive field is ever modified directly from outside its class (encapsulation respected)
- [ ] Invalid input (negative amounts, withdrawing more than balance) throws a clear, handled exception — never crashes the app
- [ ] Module works standalone (can be unit-tested without running the full console app)
- [ ] Teammate reviewed and approved the PR

---

## 🚀 Suggested order of work

1. **Day 1–2:** Member 1 builds `BankAccount`/`Customer` first — everyone else depends on this
2. **Day 2–4 (parallel):** Members 2, 3, 4 build Transactions, UI, and Data layers against the agreed class shape
3. **Day 5:** Integrate everything in `Program.cs`
4. **Day 6–7:** Member 5 runs full tests, team fixes bugs together, final polish + demo prep
