using AutoMapper;
using AzureStudents.Blazor.Api;
using AzureStudents.Blazor.Enums;
using AzureStudents.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace AzureStudents.Blazor.Components.Student;

/// <summary>
/// A component that lists students and adds functionality to add, edit and delete students. 
/// </summary>
public partial class StudentTable : ComponentBase
{
    #region Fields

    /// <summary>
    /// A collection of errors returned from the API.
    /// </summary>
    private List<MessageViewModel> _apiErrors = new();

    /// <summary>
    /// True if the component have been initialized. 
    /// </summary>
    private bool _isInitialized;

    /// <summary>
    /// Contains the students that was fetched from the database. 
    /// </summary>
	public ListViewModel<StudentViewModel> _students = new();

    #endregion

    #region Property

    /// <summary>
    /// The injected Auto Mapper service. 
    /// </summary>
    [Inject]
    private IMapper AutoMapper { get; set; } = default!;

    //// <summary>
    /// The injected Students API service.
    /// </summary>
    [Inject]
    private IStudentsApiService StudentsApiService { get; set; } = default!;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Console.WriteLine("Grr");
        Debug.WriteLine("GRR");

        var result = await StudentsApiService.GetAllStudentsAsync();

        if (result.Success)
        {
            _students = new ListViewModel<StudentViewModel>(AutoMapper.Map<List<StudentViewModel>>(result.Value));
            _isInitialized = true;
        }
        else
        {
            _apiErrors = result.Errors.Select(x => new MessageViewModel(MessageType.Error, x.ErrorMessage, title: x.ErrorType)).ToList();
        }
    }

    /// <summary>
    /// Adds an empty student placeholder row that the user can create a new student from. 
    /// </summary>
    private void AddStudentRow()
    {
        _students.Models.Add(new StudentViewModel());
    }

    /// <summary>
    /// Event handler for the <see cref="StudentTableRow.AfterDeletedStudent"/> event.
    /// </summary>
    /// <param name="student">The student that was deleted.</param>
    private void AfterDeletedStudentEventHandler(StudentViewModel student)
	{
        _students.Models.Remove(student);
    }

    /// <summary>
    /// Event handler for the <see cref="StudentTableRow.OnCancelEmptyRowEdit"/> event.
    /// </summary>
    /// <param name="student">The student placeholder that was canceled.</param>
	private void OnCancelEmptyRowEditEventHandler(StudentViewModel student)
	{
        _students.Models.Remove(student);
    }

    #endregion
}
