namespace StudentGradeManager.Core;

/// <summary>
/// Holds all student records and contains every operation the app needs.
/// Both the console and the WinForms front-ends use this one class,
/// so the business logic is written (and tested) only once.
/// </summary>
public class GradeBook
{
    public const int MinGrade = 0;
    public const int MaxGrade = 100;

    // Name -> grade. Names are case-insensitive so "alice" and "Alice" are the same student.
    private readonly Dictionary<string, int> _grades =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Number of students currently stored.</summary>
    public int Count => _grades.Count;

    // ---------- Validation helpers ----------

    /// <summary>Trims the name and rejects empty names.</summary>
    public static string NormalizeName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Student name cannot be empty.");
        return name.Trim();
    }

    /// <summary>Converts text to a grade, throwing InvalidGradeException on bad input.</summary>
    public static int ParseGrade(string? input)
    {
        if (!int.TryParse(input?.Trim(), out int grade))
            throw new InvalidGradeException("Grade must be a whole number (e.g. 78).");

        ValidateGrade(grade);
        return grade;
    }

    /// <summary>Ensures the grade is within 0-100.</summary>
    public static void ValidateGrade(int grade)
    {
        if (grade < MinGrade || grade > MaxGrade)
            throw new InvalidGradeException($"Grade must be between {MinGrade} and {MaxGrade}.");
    }

    /// <summary>Maps a numeric grade to its GradeCategory.</summary>
    public static GradeCategory Classify(int grade) => grade switch
    {
        >= 80 => GradeCategory.Distinction,
        >= 65 => GradeCategory.Merit,
        >= 50 => GradeCategory.Pass,
        _ => GradeCategory.Fail
    };

    // ---------- Feature 1: Add a student ----------

    public void AddStudent(string? name, int grade)
    {
        string cleanName = NormalizeName(name);
        ValidateGrade(grade);

        // TryAdd returns false if the key already exists.
        if (!_grades.TryAdd(cleanName, grade))
            throw new DuplicateStudentException(cleanName);
    }

    // ---------- Error handling: grade for a non-existent student ----------

    /// <summary>Changes the grade of an EXISTING student; throws if they don't exist.</summary>
    public void UpdateGrade(string? name, int grade)
    {
        string cleanName = NormalizeName(name);
        ValidateGrade(grade);

        if (!_grades.ContainsKey(cleanName))
            throw new StudentNotFoundException(cleanName);

        _grades[cleanName] = grade;
    }

    public void RemoveStudent(string? name)
    {
        string cleanName = NormalizeName(name);
        if (!_grades.Remove(cleanName))
            throw new StudentNotFoundException(cleanName);
    }

    // ---------- Feature 2: Display all students ----------

    /// <summary>All students sorted alphabetically.</summary>
    public List<StudentRecord> GetAllStudents()
    {
        var list = new List<StudentRecord>();
        foreach (var pair in _grades.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase))
            list.Add(new StudentRecord(pair.Key, pair.Value));
        return list;
    }

    // ---------- Feature 3: Search for a student ----------

    /// <summary>Returns the student's record or throws StudentNotFoundException.</summary>
    public StudentRecord FindStudent(string? name)
    {
        string cleanName = NormalizeName(name);

        if (!_grades.TryGetValue(cleanName, out int grade))
            throw new StudentNotFoundException(cleanName);

        // Re-read the stored key so we show the name with its original capitalisation.
        string storedName = _grades.Keys.First(k =>
            string.Equals(k, cleanName, StringComparison.OrdinalIgnoreCase));
        return new StudentRecord(storedName, grade);
    }

    // ---------- Feature 4: Average ----------

    public double CalculateAverage()
    {
        EnsureNotEmpty();
        return _grades.Values.Average();
    }

    // ---------- Feature 5: Highest and lowest (Max() / Min() on the values) ----------

    public int HighestGrade()
    {
        EnsureNotEmpty();
        return _grades.Values.Max();
    }

    public int LowestGrade()
    {
        EnsureNotEmpty();
        return _grades.Values.Min();
    }

    /// <summary>Names of every student who scored exactly the given grade.</summary>
    public List<string> StudentsWithGrade(int grade) =>
        _grades.Where(p => p.Value == grade)
               .Select(p => p.Key)
               .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
               .ToList();

    private void EnsureNotEmpty()
    {
        if (_grades.Count == 0)
            throw new InvalidOperationException("There are no students yet. Add a student first.");
    }
}
