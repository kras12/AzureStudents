using AzureStudents.Api.Controllers;
using AzureStudents.Api.Services;
using AzureStudents.Data.Entities;
using AzureStudents.Data.Repositories;
using AzureStudents.Shared.Dto.Student;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace AzureStudents.Test.Tests.Controllers;

/// <summary>
/// Provides tests for the student controller. 
/// </summary>
public class StudentControllerTest : ControllerTestBase
{
    #region Fields

    /// <summary>
    /// The student repository. 
    /// </summary>
    private readonly IStudentRepository _studentRepository;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public StudentControllerTest()
    {
        _studentRepository = new StudentRepository(_applicationDbContext);
    }

    #endregion

    #region HelperMethods

    /// <summary>
    /// Creates a student controller with a mocked context.
    /// </summary>
    /// <param name="isUserAuthenticated">True if the user is authenticated.</param>
    /// <returns>A <see cref="Task"/> containing a <see cref="StudentController"/>.</returns>
    private StudentController CreateStudentController(bool isUserAuthenticated)
    {
        // User
        ClaimsPrincipal claimsPrincipal = CreateClaimsPrincipalMock(isUserAuthenticated).Object;

        // Controller context
        var controllerContextMock = CreateControllerContextMock(claimsPrincipal);

        // Home controller
        var authorizationServiceMock = new Mock<IUserAuthorizationService>();
        authorizationServiceMock.Setup(x => x.IsUserAuthorized(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>()))
            .ReturnsAsync(isUserAuthenticated ? true : false);
            
        var homeController = new StudentController(_studentRepository, _autoMapper, authorizationServiceMock.Object);
        homeController.ControllerContext = controllerContextMock.Object;

        return homeController;
    }

    /// <summary>
    /// Seeds the default student to the database.
    /// </summary>
    /// <returns><see cref="Task{TResult}"/> containing the new <see cref="Student"/>.</returns>
    private async Task<Student> SeedDefaultStudent()
    {
        return await _studentRepository.AddAsync(CreateDefaultStudent());
    }

    #endregion

    #region Tests

    /// <summary>
    /// Tests creating a new student while the user is authenticated.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [Fact]
    public async Task CreateStudent_UserIsAuthenticated_ShouldCreateNewRecord()
    {
        // Arrange
        var studentController = CreateStudentController(isUserAuthenticated: true);
        CreateStudentDto newStudent = _autoMapper.Map<CreateStudentDto>(CreateDefaultStudent());

        // Act
        var createStudentReponse = await studentController.Create(newStudent) as OkObjectResult;

        // Assert
        AssertOkResponse<StudentDto>(createStudentReponse, student =>
        {
            Assert.True(student.FirstName == newStudent.FirstName && student.LastName == newStudent.LastName && student.Id > 0);
        });
    }

    /// <summary>
    /// Tests creating a new student while the user is not authenticated.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [Fact]
    public async Task CreateStudent_UserIsNotAuthenticated_ShouldReturnUnathorized()
    {
        // Arrange
        var studentController = CreateStudentController(isUserAuthenticated: false);
        CreateStudentDto newStudent = _autoMapper.Map<CreateStudentDto>(CreateDefaultStudent());

        // Act
        var response = await studentController.Create(newStudent);

        // Assert
        AssertUnauthorizedResponse<StudentDto>(response);
    }

    /// <summary>
    /// Tests deleting a student while the user is authenticated.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [Fact]
    public async Task DeleteStudent_UserIsAuthenticated_ShouldDeleteRecord()
    {
        // Arrange
        var studentController = CreateStudentController(isUserAuthenticated: true);
        var newStudent = await SeedDefaultStudent();

        // Act
        var deleteStudentResponse = await studentController.Delete(newStudent.Id);

        // Assert
        AssertOkResponse<StudentDto>(deleteStudentResponse, expectsNullResponseValue: true);
        Assert.False(await _studentRepository.StudentExists(newStudent.Id));
    }

    /// <summary>
    /// Tests deleting a student while the user is not authenticated.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [Fact]
    public async Task DeleteStudent_UserIsNotAuthenticated_ShouldReturnUnathorized()
    {
        // Arrange
        var studentController = CreateStudentController(isUserAuthenticated: false);
        var newStudent = await SeedDefaultStudent();

        // Act
        var response = await studentController.Delete(newStudent.Id);

        // Assert
        AssertUnauthorizedResponse<StudentDto>(response);
    }

    /// <summary>
    /// Tests editing a student while the user is authenticated.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [Fact]
    public async Task EditStudent_UserIsAuthenticated_ShouldUpdateRecord()
    {
        // Arrange
        var studentController = CreateStudentController(isUserAuthenticated: true);
        var newStudent = await SeedDefaultStudent();

        // Act
        UpdateStudentDto updateStudentDto = _autoMapper.Map<UpdateStudentDto>(newStudent);
        updateStudentDto.FirstName = $"{newStudent.FirstName}X";
        updateStudentDto.LastName = $"{newStudent.LastName}X";

        var updateStudentResponse = await studentController.Edit(newStudent.Id, updateStudentDto);        

        // Assert
        AssertOkResponse<StudentDto>(updateStudentResponse, updatedStudent =>
        {
            Assert.True(updatedStudent.FirstName == updateStudentDto.FirstName && updatedStudent.LastName == updateStudentDto.LastName && newStudent.Id == updatedStudent.Id);
        });

        Assert.NotNull(await _studentRepository.GetByIdAsync(newStudent.Id));
    }

    /// <summary>
    /// Tests editing a student while the user is no authenticated.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [Fact]
    public async Task EditStudent_UserIsNotAuthenticated_ShoulReturnUnathorized()
    {
        // Arrange
        var studentController = CreateStudentController(isUserAuthenticated: false);
        var newStudent = await SeedDefaultStudent();

        // Act
        UpdateStudentDto updateStudentDto = _autoMapper.Map<UpdateStudentDto>(newStudent);
        updateStudentDto.FirstName = $"{newStudent.FirstName}X";
        updateStudentDto.LastName = $"{newStudent.LastName}X";

        var updateStudentResponse = await studentController.Edit(newStudent.Id, updateStudentDto);

        // Assert
        AssertUnauthorizedResponse<StudentDto>(updateStudentResponse);
    }

    /// <summary>
    /// Tests fetching a student by ID while the user is authenticated.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [Fact]
    public async Task GetStudentById_UserIsAuthenticated_ShouldFetchRecord()
    {
        // Arrange
        var studentController = CreateStudentController(isUserAuthenticated: true);
        var newStudent = await SeedDefaultStudent();

        // Act
        var fetchStudentResponse = await studentController.GetStudentById(newStudent.Id);

        // Assert
        AssertOkResponse<StudentDto>(fetchStudentResponse, student =>
        {
            Assert.True(student.Id == newStudent.Id);
        });
    }

    /// <summary>
    /// Tests fetching a student by ID while the user is not authenticated.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [Fact]
    public async Task GetStudentById_UserIsNotAuthenticated_ShoulReturnUnathorized()
    {
        // Arrange
        var studentController = CreateStudentController(isUserAuthenticated: false);
        var newStudent = await SeedDefaultStudent();

        // Act
        var fetchStudentResponse = await studentController.GetStudentById(newStudent.Id);

        // Assert
        AssertUnauthorizedResponse<StudentDto>(fetchStudentResponse);
    }

    /// <summary>
    /// Tests fetching all students while the user is authenticated and when there are no students in the database.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [Fact]
    public async Task ListStudents_UserIsAuthenticated_ShouldReturnNoRecords()
    {
        // Arrange
        var studentController = CreateStudentController(isUserAuthenticated: true);

        // Act
        var listStudentsResponse = await studentController.List();

        // Assert
        AssertOkResponse<List<StudentDto>>(listStudentsResponse, studentList =>
        {
            Assert.True(studentList.Count == 0);
        });
    }

    /// <summary>
    /// Tests fetching all students while the user is authenticated and when there are students in the database.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [Fact]
    public async Task ListStudents_UserIsAuthenticated_ShouldReturnRecords()
    {
        // Arrange
        var studentController = CreateStudentController(isUserAuthenticated: true);
        await SeedDefaultStudent();

        // Act
        var listStudentsResponse = await studentController.List();

        // Assert
        AssertOkResponse<List<StudentDto>>(listStudentsResponse, studentList =>
        {
            Assert.True(studentList.Count > 0);
        });
    }

    /// <summary>
    /// Tests fetching all students while the user is not authenticated.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [Fact]
    public async Task ListStudents_UserIsNotAuthenticated_ShoulReturnUnathorized()
    {
        // Arrange
        var studentController = CreateStudentController(isUserAuthenticated: false);

        // Act
        var listStudentsResponse = await studentController.List();

        // Assert
        AssertUnauthorizedResponse<StudentDto>(listStudentsResponse);
    }

    #endregion
}
