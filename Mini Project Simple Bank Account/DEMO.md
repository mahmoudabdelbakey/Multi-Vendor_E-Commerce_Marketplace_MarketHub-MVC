# BankVault — Demo Script

Total time: about 5 minutes. Run the app from a clean state first (delete `data/accounts.json` content or leave it as `[]`).

## 1. Intro (30 sec)

"BankVault is a console bank app built in C#. We split it into 5 parts: Models, Transactions, UI, Data and Tests, and each member owned one part."

## 2. Show the structure (30 sec)

Open Solution Explorer and show the folders: `Models`, `Transactions`, `UI`, `Data`, `tests`. Mention that `Program.cs` connects everything.

## 3. Run the app (3 min)

Start the app. A sample account `1001 / Ahmed` with balance 1000 is created on the first run.

| Step | Do this | What to say / expect |
|---|---|---|
| 1 | Choose **4** (View Balance) | Shows the starting balance. |
| 2 | Choose **1**, enter `500` | Deposit succeeds, balance becomes 1500. |
| 3 | Choose **1**, enter `-10` | Rejected with an "Invalid amount" message. The app does not crash. |
| 4 | Choose **2**, enter `200` | Withdrawal (see note below). |
| 5 | Choose **2**, enter `999999` | Rejected: not enough money. |
| 6 | Choose **1**, type `abc` | Input validation: asks again for a valid number. |
| 7 | Choose **5** (History) | Shows every operation with time, type and amount. |
| 8 | Choose **0** (Exit), run the app again, choose **4** | The balance is still there: data was saved to `accounts.json`. |

> **Note:** until the `Withdraw` condition in `TransactionService` is fixed (see Known issues in the README), steps 4 and 5 will not behave as described. Transfer (option 3) is not connected yet, so skip it in the demo.

## 4. Run the tests (1 min)

Open **Test → Test Explorer** and click **Run All**. Show the 10 green tests and explain one of them, e.g. `Deposit_NegativeAmount_Throws`: "we try to deposit a negative amount and check that the app throws an error."

## 5. Wrap up (30 sec)

Mention what we practiced: encapsulation (`Balance` has a private setter), custom exceptions, an interface for storage, and unit testing.
