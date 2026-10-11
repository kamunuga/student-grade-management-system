using StudentGradeManager.Core;

// Console front-end for the Student Grade Management System.
var gradeBook = new GradeBook();
SeedSampleData(gradeBook);

bool running = true;
while (running)
{
    ShowMenu();
    string? choice = Console.ReadLine()?.Trim();
    Console.WriteLine();

    try
    {
        switch (choice)
        {
            case "1": AddStudent(); break;
            case "2": DisplayAll(); break;
            case "3": SearchStudent(); break;
            case "4": ShowAverage(); break;
            case "5": ShowHighestAndLowest(); break;
            case "6": UpdateGrade(); break;
            case "0": running = false; Console.WriteLine("Goodbye!"); break;
            default: Console.WriteLine("Invalid option. Please choose 0-6."); break;
        }
    }
    // Each custom exception gives the user a clear, friendly message.
    catch (StudentNotFoundException ex) { Console.WriteLine($"Not found: {ex.Message}"); }
    catch (DuplicateStudentException ex) { Console.WriteLine($"Duplicate: {ex.Message}"); }
    catch (InvalidGradeException ex) { Console.WriteLine($"Invalid grade: {ex.Message}"); }
    catch (ArgumentException ex) { Console.WriteLine($"Invalid input: {ex.Message}"); }
    catch (InvalidOperationException ex) { Console.WriteLine($"Cannot do that yet: {ex.Message}"); }
    catch (Exception ex) { Console.WriteLine($"Unexpected error: {ex.Message}"); }

    if (running)
    {
        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }
}

// ---------------- Menu actions ----------------

void ShowMenu()
{
    Console.Clear();
    Console.WriteLine("=== Student Grade Management System ===");
    Console.WriteLine("1. Add a student");
    Console.WriteLine("2. Display all students");
    Console.WriteLine("3. Search for a student");
    Console.WriteLine("4. Calculate average grade");
    Console.WriteLine("5. Show highest and lowest grades");
    Console.WriteLine("6. Update a student's grade");
    Console.WriteLine("0. Exit");
    Console.Write("\nChoose an option: ");
}

void AddStudent()
{
    string name = Prompt("Student name: ");
    int grade = GradeBook.ParseGrade(Prompt("Grade (0-100): "));
    gradeBook.AddStudent(name, grade);
    Console.WriteLine($"Added {name.Trim()} with grade {grade}.");
}

void UpdateGrade()
{
    string name = Prompt("Student name: ");
    int grade = GradeBook.ParseGrade(Prompt("New grade (0-100): "));
    gradeBook.UpdateGrade(name, grade); // throws StudentNotFoundException if missing
    Console.WriteLine($"Updated {name.Trim()}'s grade to {grade}.");
}

void DisplayAll()
{
    var students = gradeBook.GetAllStudents();
    if (students.Count == 0)
    {
        Console.WriteLine("No students recorded yet.");
        return;
    }

    Console.WriteLine($"{"Name",-20}{"Grade",6}  Category");
    Console.WriteLine(new string('-', 40));
    foreach (StudentRecord s in students)
        Console.WriteLine($"{s.Name,-20}{s.Grade,6}  {s.Category}");
}

void SearchStudent()
{
    StudentRecord s = gradeBook.FindStudent(Prompt("Name to search for: "));
    Console.WriteLine($"{s.Name}: {s.Grade} ({s.Category})");
}

void ShowAverage()
{
    Console.WriteLine($"Average grade: {gradeBook.CalculateAverage():F2}");
}

void ShowHighestAndLowest()
{
    int highest = gradeBook.HighestGrade();
    int lowest = gradeBook.LowestGrade();
    Console.WriteLine($"Highest grade: {highest} ({string.Join(", ", gradeBook.StudentsWithGrade(highest))})");
    Console.WriteLine($"Lowest grade:  {lowest} ({string.Join(", ", gradeBook.StudentsWithGrade(lowest))})");
}

// ---------------- Helpers ----------------

string Prompt(string message)
{
    Console.Write(message);
    return Console.ReadLine() ?? string.Empty;
}

void SeedSampleData(GradeBook book)
{
    book.AddStudent("Alice", 88);
    book.AddStudent("Brian", 72);
    book.AddStudent("Chloe", 45);
}
