namespace StudentGradeManager.Core;

/// <summary>Thrown when a student name is looked up or updated but does not exist.</summary>
public class StudentNotFoundException : Exception
{
    public StudentNotFoundException(string name)
        : base($"Student '{name}' was not found.") { }
}

/// <summary>Thrown when adding a student whose name is already in the grade book.</summary>
public class DuplicateStudentException : Exception
{
    public DuplicateStudentException(string name)
        : base($"Student '{name}' already exists. Use 'update grade' to change their grade.") { }
}

/// <summary>Thrown when a grade is not a whole number between 0 and 100.</summary>
public class InvalidGradeException : Exception
{
    public InvalidGradeException(string message) : base(message) { }
}
