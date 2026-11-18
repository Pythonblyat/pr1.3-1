using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

// Завдання 3.1. Система «Університет»: асоціація, агрегація, композиція

// Клас кімнати — частина будівлі (для композиції)
public class Room
{
    public string Number { get; set; }
    public string Type { get; set; }
    public int Capacity { get; set; }

    public Room(string number, string type, int capacity)
    {
        Number = number;
        Type = type;
        Capacity = capacity;
    }

    public override string ToString() => $"Аудиторія {Number} ({Type}, {Capacity} місць)";
}

// Кафедра — існує лише як частина факультету (КОМПОЗИЦІЯ: Faculty *-- Department)
public class Department
{
    public string Name { get; set; }
    public string HeadName { get; set; }

    public Department(string name, string headName)
    {
        Name = name;
        HeadName = headName;
    }

    public override string ToString() => $"Кафедра {Name} (завідувач: {HeadName})";
}

// Професор — існує незалежно від факультету (АГРЕГАЦІЯ: Faculty o-- Professor)
public class Professor
{
    public string Name { get; set; }
    public string Degree { get; set; }

    // АСОЦІАЦІЯ: професор «знає» кафедру, до якої належить (Professor → Department),
    // але не володіє нею і не керує її життєвим циклом
    public Department? Department { get; set; }

    public Professor(string name, string degree, Department? department = null)
    {
        Name = name;
        Degree = degree;
        Department = department;
    }

    public override string ToString() =>
        $"{Name}, {Degree}, кафедра: {Department?.Name ?? "—"}";
}

// Факультет: композиція з Department та агрегація з Professor
public class Faculty
{
    private string _name;
    private List<Department> _departments; // Композиція — створюються всередині
    private List<Professor> _professors;   // Агрегація — передаються ззовні

    public Faculty(string name)
    {
        _name = name;
        _departments = new List<Department>();
        _professors = new List<Professor>();
    }

    public int ProfessorCount => _professors.Count;

    // КОМПОЗИЦІЯ: об'єкт Department створюється (new) всередині Faculty,
    // ззовні передаються лише дані. Знищення факультету знищує і кафедри.
    public void CreateDepartment(string name, string headName)
    {
        _departments.Add(new Department(name, headName));
    }

    // Пошук кафедри за назвою (щоб можна було пов'язати з нею професора)
    public Department? FindDepartment(string name)
    {
        foreach (Department d in _departments)
        {
            if (d.Name == name)
                return d;
        }
        return null;
    }

    // АГРЕГАЦІЯ: готовий об'єкт Professor створено ЗОВНІ й передано в Faculty.
    // Faculty лише зберігає посилання, власником професора він не є.
    public void AddProfessor(Professor professor)
    {
        if (professor == null)
            throw new ArgumentNullException(nameof(professor));
        if (!_professors.Contains(professor))
            _professors.Add(professor);
    }

    // Видалення з факультету НЕ знищує об'єкт професора
    public bool RemoveProfessor(Professor professor)
    {
        return _professors.Remove(professor);
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Факультет: {_name}");
        Console.WriteLine("  Кафедри (композиція):");
        foreach (Department d in _departments)
            Console.WriteLine($"    - {d}");
        Console.WriteLine("  Професори (агрегація):");
        foreach (Professor p in _professors)
            Console.WriteLine($"    - {p}");
    }
}

// Будівля — композиція з Room (кімнати не існують без будівлі)
public class Building
{
    private string _name;
    private List<Room> _rooms;

    public Building(string name)
    {
        _name = name;
        _rooms = new List<Room>();
    }

    // КОМПОЗИЦІЯ: кімната створюється всередині будівлі
    public void AddRoom(string number, string type, int capacity)
    {
        _rooms.Add(new Room(number, type, capacity));
    }

    // Пошук кімнати за номером (null, якщо не знайдено)
    public Room? FindRoom(string number)
    {
        foreach (Room room in _rooms)
        {
            if (room.Number == number)
                return room;
        }
        return null;
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Будівля: {_name}");
        foreach (var room in _rooms)
            Console.WriteLine($"  {room}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        // TODO 1: факультет
        Faculty faculty = new Faculty("Комп'ютерних наук");

        // TODO 2: кафедри (композиція — створюються всередині факультету)
        faculty.CreateDepartment("Програмної інженерії", "Мельник О. І.");
        faculty.CreateDepartment("Прикладної математики", "Коваль С. П.");

        // TODO 3: професори створюються окремо (існують незалежно)
        Department? seDept = faculty.FindDepartment("Програмної інженерії");
        Department? mathDept = faculty.FindDepartment("Прикладної математики");

        Professor prof1 = new Professor("Мельник Олег Іванович", "д.т.н.", seDept);
        Professor prof2 = new Professor("Шевчук Ірина Петрівна", "к.ф.-м.н.", mathDept);
        Professor prof3 = new Professor("Бондаренко Андрій Сергійович", "к.т.н.", seDept);

        // TODO 4: додаємо професорів до факультету (агрегація)
        faculty.AddProfessor(prof1);
        faculty.AddProfessor(prof2);
        faculty.AddProfessor(prof3);

        // TODO 5: будівля та аудиторії (композиція)
        Building building = new Building("Головний корпус");
        building.AddRoom("101", "Лекційна", 120);
        building.AddRoom("102", "Лекційна", 80);
        building.AddRoom("201", "Комп'ютерний клас", 30);
        building.AddRoom("202", "Лабораторія", 25);

        // TODO 6: виводимо інформацію
        Console.WriteLine("=== Факультет ===");
        faculty.DisplayInfo();

        Console.WriteLine("\n=== Будівля ===");
        building.DisplayInfo();

        Room? found = building.FindRoom("201");
        Console.WriteLine("Пошук аудиторії 201: " + found);
        Room? missing = building.FindRoom("999");
        Console.WriteLine(missing == null
            ? "Пошук аудиторії 999: аудиторію не знайдено"
            : "Пошук аудиторії 999: " + missing);

        // TODO 7: професор існує незалежно від факультету
        Console.WriteLine("\n=== Незалежність професора від факультету ===");
        Console.WriteLine($"Професорів на факультеті: {faculty.ProfessorCount}");
        faculty.RemoveProfessor(prof2);
        Console.WriteLine($"Після видалення професора {prof2.Name} " +
                          $"з факультету їх залишилось: {faculty.ProfessorCount}");
        Console.WriteLine("Об'єкт професора продовжує існувати: " + prof2);
    }
}