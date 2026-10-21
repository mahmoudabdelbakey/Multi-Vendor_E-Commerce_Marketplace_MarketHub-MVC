using BankVault.Data;
using BankVault.Models;
using BankVault.Transactions;
using BankVault.UI;



// نعمل object من الكلاس. الكتابة بنوع الـ interface بتوضح إننا بنستخدم "العقد" بس
IAccountRepository repo = new FileAccountRepository();

var acc1 = new BankAccount("5010", "Mohamed", 10000, AccountType.Savings);
var acc2 = new BankAccount("5011", "Ali", 5000, AccountType.Current);

// Add: بيضيف حساب ويحفظه في الملف على طول
repo.Add(acc1);
repo.Add(acc2);
// repo.Add(acc1);  // لو كررتها يرمي Exception("Account already exists")

// GetAll: بيرجّع كل الحسابات
foreach (BankAccount a in repo.GetAll())
    Console.WriteLine(a);

// GetByAccountNumber: بيدوّر بالرقم، وبيرجّع null لو مش موجود
BankAccount? found = repo.GetByAccountNumber("5001");
Console.WriteLine(found);                              // يطبع حساب Ahmed
Console.WriteLine(repo.GetByAccountNumber("0000"));    // فاضي (null)

// Update: عدّل الحساب الأول وبعدين ابعته
acc1.Deposit(250);
repo.Update(acc1);                                     // بيستبدله ويحفظ (الرصيد 1250)

// SaveChanges: بيكتب كل اللي في الميموري في ملف JSON
repo.SaveChanges();

// LoadData: بيرجّع يقرا من الملف ويرجّع الحسابات (يثبت إن الحفظ اشتغل)
repo.LoadData();
Console.WriteLine(repo.GetByAccountNumber("5001"));    // لسه رصيده 1250

// Delete: بيمسح حساب بالرقم ويحفظ
repo.Delete("5002");
Console.WriteLine(repo.GetAll().Count);                // 1


// 1. Load saved accounts from the JSON file
var repository = new FileAccountRepository();

// 2. If there are no accounts yet, create a sample one
if (repository.GetAll().Count == 0)
{
    repository.Add(new BankAccount("5010", "Ahmed", 1000, AccountType.Savings));
}

// 3. Use the first account in the menu
BankAccount account = repository.GetAll()[0];

// 4. Create the service and run the menu
var service = new TransactionService(new TransactionHistory());
var menu = new MainMenu(service, account);
menu.Run();

// 5. Save the new balance when the user exits
repository.SaveChanges();
