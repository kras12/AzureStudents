using AzureStudents.Data.Entities;
using AzureStudents.Data.Repositories;

namespace AzureStudents.Test.Tests.Repositories;

/// <summary>
/// Test class for the student repository. 
/// </summary>
public class StudentRepositoryTest : TestBase
{
    #region Fields

    /// <summary>
    /// The student repository. 
    /// </summary>
    IStudentRepository _studentRepository { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public StudentRepositoryTest()
    {
        _studentRepository = new StudentRepository(_applicationDbContext);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Tests adding a student with valid details. 
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    [Fact]
    public async Task AddStudent_WithValidDetails_ShouldCreateNewRecord()
    {
        // Arrange
        var newStudent = CreateDefaultStudent();

        // Act
        var createdStudent = await _studentRepository.AddAsync(newStudent);

        // Assert
        Assert.Equal(createdStudent.FirstName, newStudent.FirstName);
        Assert.Equal(createdStudent.LastName, newStudent.LastName);
    }

    /// <summary>
    /// Tests whether any students exists in the database when there are no students in the database.
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    [Fact]
    public async Task AnyStudentExists_ShouldReturnFalse()
    {
        // Arrange

        // Act
        bool haveAnyStudents = await _studentRepository.AnyAsync();

        // Assert
        Assert.False(haveAnyStudents);
    }

    /// <summary>
    /// Tests whether any students exists in the database when there are students in the database. 
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    [Fact]
    public async Task AnyStudentExists_ShouldReturnTrue()
    {
        // Arrange
        var newStudent = CreateDefaultStudent();

        var createdStudent = await _studentRepository.AddAsync(newStudent);

        // Act
        var haveAnyStudents = await _studentRepository.AnyAsync();

        // Assert
        Assert.True(haveAnyStudents);
    }

    /// <summary>
    /// Tests deleting a student from the database. 
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    [Fact]
    public async Task DeleteStudent_ShouldRemoveTheRecord()
    {
        // Arrange
        var newStudent = CreateDefaultStudent();

        var createdStudent = await _studentRepository.AddAsync(newStudent);

        // Act
        await _studentRepository.DeleteAsync(createdStudent.Id);
        var deletedStudent = await _studentRepository.GetByIdAsync(createdStudent.Id);

        // Assert
        Assert.Null(deletedStudent);
    }

    /// <summary>
    /// Tests fetching all students when there are no students in the database. 
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    [Fact]
    public async Task GetAllStudents_ShouldReturnEmptyCollection()
    {
        // Arrange

        // Act
        var fetchedStudents = await _studentRepository.GetAll();

        // Assert
        Assert.NotNull(fetchedStudents);
        Assert.Empty(fetchedStudents);
    }

    /// <summary>
    /// Tests fetching all students when there are students in the database. 
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    [Fact]
    public async Task GetAllStudents_ShouldReturnFilledCollection()
    {
        // Arrange
        var newStudent = CreateDefaultStudent();

        var createdStudent = await _studentRepository.AddAsync(newStudent);

        // Act
        var fetchedStudents = await _studentRepository.GetAll();

        // Assert
        Assert.NotNull(fetchedStudents);
        Assert.NotEmpty(fetchedStudents);
    }

    /// <summary>
    /// Tests fetching a student by ID.
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    [Fact]
    public async Task GetStudentById_ShouldReturnCorrectStudent()
    {
        // Arrange
        var newStudent = CreateDefaultStudent();
        var createdStudent = await _studentRepository.AddAsync(newStudent);

        // Act
        var fetchedStudent = await _studentRepository.GetByIdAsync(createdStudent.Id);

        // Assert
        Assert.NotNull(fetchedStudent);
        Assert.Equal(fetchedStudent.FirstName, newStudent.FirstName);
        Assert.Equal(fetchedStudent.LastName, newStudent.LastName);
    }

    /// <summary>
    /// Tests whether a given student exists in the database when the student doesn't exists in the database. 
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    [Fact]
    public async Task StudentExists_ShouldReturnFalse()
    {
        // Arrange

        // Act
        bool studentExists = await _studentRepository.StudentExists(999);

        // Assert
        Assert.False(studentExists);
    }

    /// <summary>
    /// Tests whether a given student exists in the database when the student does exists in the database. 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task StudentExists_ShouldReturnTrue()
    {
        // Arrange
        var newStudent = CreateDefaultStudent();
        var createdStudent = await _studentRepository.AddAsync(newStudent);

        // Act
        bool studentExists = await _studentRepository.StudentExists(createdStudent.Id);

        // Assert
        Assert.True(studentExists);
    }

    /// <summary>
    /// Tests updating a student in the database. 
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    [Fact]
    public async Task UpdateStudent_ShouldUpdateAllProperties()
    {
        // Arrange
        var newStudent = CreateDefaultStudent();

        var createdStudent = await _studentRepository.AddAsync(newStudent);

        // Act
        var fetchedStudent = await _studentRepository.GetByIdAsync(createdStudent.Id);
        fetchedStudent!.FirstName = "Clark";
        fetchedStudent.LastName = "Kent";
        await _studentRepository.UpdateAsync(fetchedStudent);
        var updatedStudent = await _studentRepository.GetByIdAsync(createdStudent.Id);

        // Assert
        Assert.NotNull(updatedStudent);
        Assert.NotEqual(updatedStudent.FirstName, createdStudent.FirstName);
        Assert.NotEqual(updatedStudent.LastName, createdStudent.LastName);
        Assert.Equal(updatedStudent.FirstName, fetchedStudent.FirstName);
        Assert.Equal(updatedStudent.LastName, fetchedStudent.LastName);
    }

    #endregion

    #region HelperMethods

    /// <summary>
    /// Creates the default student. 
    /// </summary>
    /// <returns><see cref="Student"/></returns>
    private Student CreateDefaultStudent()
    {
        return new Student()
        {
            FirstName = "Kalle",
            LastName = "Anka",
        };
    }

    #endregion
}
