# Student Grade Management System (C#)

A C# application for managing student records and grades. It has **two front-ends** that share one core library:

| Project | What it is |
|---|---|
| `StudentGradeManager.Core` | Class library with all the logic (`GradeBook`, exceptions, enum, struct) |
| `StudentGradeManager.ConsoleApp` | Menu-driven console app |
| `StudentGradeManager.WinForms` | Windows Forms GUI (Windows only) |

## Features

- Add a student (name + grade) stored in a `Dictionary<string, int>`
- Display all students with their grade category
- Search for a student (friendly message if not found)
- Calculate the average grade
- Find the highest and lowest grades using `Max()` and `Min()` on the dictionary values
- Update a grade, and remove a student
- Error handling for non-existent students, duplicate names, empty names, and invalid grades (non-numeric or outside 0-100)
- `GradeCategory` enum (Fail / Pass / Merit / Distinction) and a `StudentRecord` struct

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (check with `dotnet --version`)
- Windows is required for the WinForms app only; the console app runs anywhere.

## How to run

Clone the repo and move into it:

```bash
git clone https://github.com/<your-username>/<your-repo>.git
cd <your-repo>
```

**Console app**

```bash
dotnet run --project StudentGradeManager.ConsoleApp
```

**Windows Forms app (Windows only)**

```bash
dotnet run --project StudentGradeManager.WinForms
```

Or open `StudentGradeManager.sln` in Visual Studio and press F5 (set the startup project first).

## Grade categories

| Range | Category |
|---|---|
| 80 - 100 | Distinction |
| 65 - 79 | Merit |
| 50 - 64 | Pass |
| 0 - 49 | Fail |

## Notes

- Student names are case-insensitive (`alice` and `Alice` are the same student).
- Sample students (Alice, Brian, Chloe) are loaded at startup so you can try the features immediately.
