namespace AzureStudents.Shared.Dto.Student;

/// <summary>
/// Represents a student in the database. 
/// </summary>
public class StudentDto
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
