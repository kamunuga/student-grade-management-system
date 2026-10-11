namespace StudentGradeManager.Core;

/// <summary>
/// A lightweight, immutable snapshot of one student's record.
/// Implemented as a struct (value type) as required by the assignment.
/// </summary>
public readonly record struct StudentRecord(string Name, int Grade)
{
    /// <summary>The grade band this student's score falls into.</summary>
    public GradeCategory Category => GradeBook.Classify(Grade);
}
