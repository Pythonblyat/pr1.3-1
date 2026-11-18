using System;
using System.Globalization;
using System.Text;

// Завдання 1.1. Клас BankAccount: поля, конструктори, методи

public class Program
{
    // ----- Поля (приватні — інкапсуляція) -----
    private string _accountNumber;
    private string _ownerName;
    private decimal _balance;

    // Статичне поле належить класу, а не окремому об'єкту:
    // його значення спільне для всіх створених рахунків
    private static int _totalAccounts = 0;

    // ----- Конструктори -----

    // Конструктор за замовчуванням: значення за замовчуванням
    public Program()
    {
        _accountNumber = "0000";
        _ownerName = "Невідомо";
        _balance = 0m;
        _totalAccounts++;
    }

    // Параметризований конструктор
    public Program(string accountNumber, string ownerName, decimal initialBalance)
    {
        _accountNumber = accountNumber;
        _ownerName = ownerName;
        // Початковий баланс не може бути від'ємним
        _balance = initialBalance < 0 ? 0m : initialBalance;
        _totalAccounts++;
    }

    // Конструктор копіювання: створює НЕЗАЛЕЖНУ копію рахунку
    public Program(Program other)
    {
        _accountNumber = other._accountNumber;
        _ownerName = other._ownerName;
        _balance = other._balance;
        _totalAccounts++; // копія — теж новий об'єкт
    }

    // ----- Методи -----

    // Поповнення рахунку: сума має бути додатною
    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Помилка: сума поповнення має бути більшою за 0.");
            return;
        }
        _balance += amount;
    }

    // Зняття коштів: true — операцію виконано, false — відмовлено
    public bool Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Помилка: сума зняття має бути більшою за 0.");
            return false;
        }
        if (amount > _balance)
        {
            Console.WriteLine("Помилка: недостатньо коштів на рахунку.");
            return false;
        }
        _balance -= amount;
        return true;
    }

    // Статичний метод викликається через ім'я класу: BankAccount.GetTotalAccounts()
    public static int GetTotalAccounts()
    {
        return _totalAccounts;
    }

    // Перевизначення ToString()
    public override string ToString()
    {
        return $"Рахунок: {_accountNumber}, Власник: {_ownerName}, " +
               $"Баланс: {_balance:F2} грн";
    }
}

class Program
{
    // Допоміжний метод для виводу результату операції
    static void ShowResult(string operation, bool success)
    {
        Console.WriteLine($"{operation}: {(success ? "успішно" : "відмовлено")}");
    }

    static void Main(string[] args)
    {
        // Кирилиця в консолі та крапка як роздільник дробової частини (1000.00)
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        Console.WriteLine("=== Створення об'єктів ===");
        Program acc1 = new Program();                 // за замовчуванням
        Program acc2 = new Program("1234", "Іван Петренко", 1000m);
        Program acc3 = new Program("5678", "Марія Коваленко", 2500.50m);
        Program acc4 = new Program(acc2);             // копіювання

        Console.WriteLine("1) За замовчуванням: " + acc1);
        Console.WriteLine("2) Параметризований: " + acc2);
        Console.WriteLine("3) Параметризований: " + acc3);
        Console.WriteLine("4) Копіювання:       " + acc4);
        Console.WriteLine($"\nВсього створено рахунків: {Program.GetTotalAccounts()}");

        Console.WriteLine("\n=== Deposit ===");
        acc2.Deposit(500m);
        Console.WriteLine(acc2);
        acc2.Deposit(-100m); // некоректна сума — повідомлення про помилку

        Console.WriteLine("\n=== Withdraw ===");
        ShowResult("Зняття 200 грн", acc2.Withdraw(200m));
        Console.WriteLine(acc2);
        ShowResult("Зняття 10000 грн", acc3.Withdraw(10000m)); // більше за баланс
        ShowResult("Зняття -50 грн", acc3.Withdraw(-50m));     // від'ємна сума
        Console.WriteLine(acc3);

        Console.WriteLine("\n=== Незалежність копії ===");
        Console.WriteLine("Оригінал (acc2): " + acc2);
        Console.WriteLine("Копія    (acc4): " + acc4);
    }
}