using StudentGradeManager.Core;

namespace StudentGradeManager.WinForms;

/// <summary>
/// Windows Forms front-end. The UI is built in code (no designer file),
/// and every operation is delegated to the shared GradeBook class.
/// </summary>
public class MainForm : Form
{
    private readonly GradeBook _gradeBook = new();

    private readonly TextBox _nameBox = new() { Width = 180 };
    private readonly TextBox _gradeBox = new() { Width = 80 };
    private readonly DataGridView _grid = new();
    private readonly Label _statusLabel = new() { AutoSize = true };
    private readonly Label _statsLabel = new() { AutoSize = true };

    public MainForm()
    {
        Text = "Student Grade Management System";
        Width = 720;
        Height = 560;
        StartPosition = FormStartPosition.CenterScreen;

        BuildLayout();

        // Sample data so the grid isn't empty on first launch.
        _gradeBook.AddStudent("Alice", 88);
        _gradeBook.AddStudent("Brian", 72);
        _gradeBook.AddStudent("Chloe", 45);
        RefreshGrid();
    }

    private void BuildLayout()
    {
        // --- Input row ---
        var inputPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top, Height = 45, Padding = new Padding(8)
        };
        inputPanel.Controls.Add(new Label { Text = "Name:", AutoSize = true, Margin = new Padding(0, 6, 0, 0) });
        inputPanel.Controls.Add(_nameBox);
        inputPanel.Controls.Add(new Label { Text = "Grade (0-100):", AutoSize = true, Margin = new Padding(10, 6, 0, 0) });
        inputPanel.Controls.Add(_gradeBox);

        // --- Button row ---
        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top, Height = 45, Padding = new Padding(8)
        };
        buttonPanel.Controls.Add(MakeButton("Add Student", (_, _) => Run(AddStudent)));
        buttonPanel.Controls.Add(MakeButton("Update Grade", (_, _) => Run(UpdateGrade)));
        buttonPanel.Controls.Add(MakeButton("Search", (_, _) => Run(SearchStudent)));
        buttonPanel.Controls.Add(MakeButton("Remove", (_, _) => Run(RemoveStudent)));
        buttonPanel.Controls.Add(MakeButton("Show All", (_, _) => Run(() => { RefreshGrid(); Say("Showing all students."); })));
        buttonPanel.Controls.Add(MakeButton("Statistics", (_, _) => Run(ShowStatistics)));

        // --- Grid ---
        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.Columns.Add("Name", "Name");
        _grid.Columns.Add("Grade", "Grade");
        _grid.Columns.Add("Category", "Category");

        // --- Bottom status area ---
        var bottomPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom, Height = 70, Padding = new Padding(8),
            FlowDirection = FlowDirection.TopDown
        };
        bottomPanel.Controls.Add(_statsLabel);
        bottomPanel.Controls.Add(_statusLabel);

        // Dock order matters: Fill first, then the edges.
        Controls.Add(_grid);
        Controls.Add(bottomPanel);
        Controls.Add(buttonPanel);
        Controls.Add(inputPanel);
    }

    private static Button MakeButton(string text, EventHandler onClick)
    {
        var button = new Button { Text = text, AutoSize = true };
        button.Click += onClick;
        return button;
    }

    // ---------- Central error handling ----------

    /// <summary>Runs an action and converts any exception into a friendly message box.</summary>
    private void Run(Action action)
    {
        try
        {
            action();
        }
        catch (StudentNotFoundException ex) { Warn("Student not found", ex.Message); }
        catch (DuplicateStudentException ex) { Warn("Duplicate student", ex.Message); }
        catch (InvalidGradeException ex) { Warn("Invalid grade", ex.Message); }
        catch (ArgumentException ex) { Warn("Invalid input", ex.Message); }
        catch (InvalidOperationException ex) { Warn("Nothing to calculate", ex.Message); }
        catch (Exception ex) { Warn("Unexpected error", ex.Message); }
    }

    // ---------- Actions ----------

    private void AddStudent()
    {
        int grade = GradeBook.ParseGrade(_gradeBox.Text);
        _gradeBook.AddStudent(_nameBox.Text, grade);
        RefreshGrid();
        Say($"Added {_nameBox.Text.Trim()} with grade {grade}.");
        ClearInputs();
    }

    private void UpdateGrade()
    {
        int grade = GradeBook.ParseGrade(_gradeBox.Text);
        _gradeBook.UpdateGrade(_nameBox.Text, grade);
        RefreshGrid();
        Say($"Updated {_nameBox.Text.Trim()}'s grade to {grade}.");
        ClearInputs();
    }

    private void RemoveStudent()
    {
        _gradeBook.RemoveStudent(_nameBox.Text);
        RefreshGrid();
        Say($"Removed {_nameBox.Text.Trim()}.");
        ClearInputs();
    }

    private void SearchStudent()
    {
        StudentRecord s = _gradeBook.FindStudent(_nameBox.Text);

        // Show only the matching student in the grid.
        _grid.Rows.Clear();
        _grid.Rows.Add(s.Name, s.Grade, s.Category);
        Say($"Found {s.Name}: {s.Grade} ({s.Category}). Click 'Show All' to see everyone.");
    }

    private void ShowStatistics()
    {
        double average = _gradeBook.CalculateAverage();
        int highest = _gradeBook.HighestGrade();
        int lowest = _gradeBook.LowestGrade();
        _statsLabel.Text = $"Average: {average:F2}   |   Highest: {highest}   |   Lowest: {lowest}";
        Say("Statistics updated.");
    }

    // ---------- Helpers ----------

    private void RefreshGrid()
    {
        _grid.Rows.Clear();
        foreach (StudentRecord s in _gradeBook.GetAllStudents())
            _grid.Rows.Add(s.Name, s.Grade, s.Category);
    }

    private void ClearInputs()
    {
        _nameBox.Clear();
        _gradeBox.Clear();
        _nameBox.Focus();
    }

    private void Say(string message) => _statusLabel.Text = message;

    private static void Warn(string title, string message) =>
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
