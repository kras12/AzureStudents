namespace AzureStudents.Blazor.ViewModels;

/// <summary>
/// Viewmodel for a student. 
/// </summary>
public class StudentViewModel
{
    /// <summary>
    /// The first name of the student.
    /// </summary>
    public string FirstName { get; set; } = "";

    /// <summary>
    /// The ID of the student.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The last name of the student. 
    /// </summary>
    public string LastName { get; set; } = "";
}
