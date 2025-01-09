namespace AzureStudents.Shared.Dto.Student;

/// <summary>
/// Represents a student in the database. 
/// </summary>
public class UpdateStudentDto
{
    /// <summary>
    /// The first name of the student.
    /// </summary>
    public string FirstName { get; set; } = "";

    /// <summary>
    /// The last name of the student. 
    /// </summary>
    public string LastName { get; set; } = "";
}
