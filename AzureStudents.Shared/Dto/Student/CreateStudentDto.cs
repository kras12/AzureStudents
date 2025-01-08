namespace AzureStudents.Shared.Dto.Student;

/// <summary>
/// Represents a student in the database. 
/// </summary>
public class CreateStudentDto
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
