using AutoMapper;
using AzureStudents.Blazor.Api;
using AzureStudents.Blazor.Enums;
using AzureStudents.Blazor.ViewModels;
using AzureStudents.Shared.Dto.Api;
using AzureStudents.Shared.Dto.Student;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AzureStudents.Blazor.Components.Student;

/// <summary>
/// A component that represents the rows in the <see cref="StudentTable"/> component. 
/// </summary>
public partial class StudentTableRow : ComponentBase
{
	#region Fields    

    /// <summary>
    /// A collection of API errors. 
    /// </summary>
	private List<MessageViewModel> _apiErrors = new();

    /// <summary>
    /// Represents the student while in edit mode. 
    /// </summary>
	private StudentViewModel _editStudent = new();

    /// <summary>
    /// True if the student edit mode is enabled. 
    /// </summary>
	private bool _isInEditMode;

    /// <summary>
    /// True if the component is busy processing something. 
    /// </summary>
    private bool _isProcessing;

    #endregion

    #region Events

    /// <summary>
    /// An event callback for an event that triggers after a student have been deleted. 
    /// </summary>
	[Parameter]
    public EventCallback<StudentViewModel> AfterDeletedStudent { get; set; } = default!;

    /// <summary>
    /// An event callback for an event that triggers when the user cancels the creation of a new student (empty placeholder row). 
    /// </summary>
    [Parameter]
    public EventCallback<StudentViewModel> OnCancelEmptyRowEdit { get; set; } = default!;

    #endregion

    #region Properties

    /// <summary>
    /// The injected AutoMapper. 
    /// </summary>
    [Inject]
    private IMapper AutoMapper { get; set; } = default!;

    /// <summary>
    /// The injected JavaScript runtime. 
    /// </summary>
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// The student to display.
    /// </summary>
	[Parameter]
	public StudentViewModel Student { get; set; } = new();

    /// <summary>
    /// The injected Students API service. 
    /// </summary>
	[Inject]
	private IStudentsApiService StudentsApiService { get; set; } = default!;

    #endregion

    #region Methods

    /// <summary>
    /// Cancels the edit of the student. If cancelling the creation of a new student the <see cref="OnCancelEmptyRowEdit"/> event is triggered. 
    /// </summary>
    private void CancelEdit()
    {
        if (Student.Id == 0 && OnCancelEmptyRowEdit.HasDelegate)
        {
            OnCancelEmptyRowEdit.InvokeAsync(Student);
        }

        EndEditMode();
    }

    /// <summary>
    /// Deletes the student from the database and triggers the <see cref="AfterDeletedStudent"/> event. 
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    /// <exception cref="InvalidOperationException"></exception>
	private async Task DeleteStudent()
    {
        if (Student.Id <= 0)
        {
            throw new InvalidOperationException("Student ID must be a positive value.");
        }

        try
        {

            if (await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this student?"))
            {
                _apiErrors.Clear();
                _isProcessing = true;
                var result = await StudentsApiService.DeleteStudentAsync(Student.Id);

                if (result.Success)
                {
                    if (AfterDeletedStudent.HasDelegate)
                    {
                        await AfterDeletedStudent.InvokeAsync(Student);
                    }

                    _isInEditMode = false;
                }
                else
                {
                    _apiErrors = result.Errors.Select(x => new MessageViewModel(MessageType.Error, x.ErrorMessage, title: x.ErrorType)).ToList();
                }
            }
        }
        finally
        {
            _isProcessing = false;
        }
    }

    /// <summary>
    /// Ends the student edit mode. 
    /// </summary>
    private void EndEditMode()
    {
#if !DEBUG
		if (!_isInEditMode)
		{
			throw new InvalidOperationException("Edit mode is already inactive.");
		}
#endif

        _isInEditMode = false;
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (Student.Id == 0)
        {
            StartEditMode();
        }
    }

    /// <summary>
    /// Saves the edited student to the database. 
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task SaveStudentAsync()
	{
        try
        {
            if (!_isInEditMode)
            {
                throw new InvalidOperationException("Must be in edit mode to save student data.");
            }

            _apiErrors.Clear();
            ApiResponseDto<StudentDto> result;
            _isProcessing = true;


            if (Student.Id > 0)
            {
                result = await StudentsApiService.UpdateStudentAsync(_editStudent.Id, AutoMapper.Map<UpdateStudentDto>(_editStudent));
            }
            else
            {
                result = await StudentsApiService.CreateStudentAsync(AutoMapper.Map<CreateStudentDto>(_editStudent));
            }

            if (result.Success)
            {
                AutoMapper.Map(result.Value, Student);
                EndEditMode();
            }
            else
            {
                _apiErrors = result.Errors.Select(x => new MessageViewModel(MessageType.Error, x.ErrorMessage, title: x.ErrorType)).ToList();
            }
        }
        finally
        {
            _isProcessing = false;
        }
    }    

    /// <summary>
    /// Starts the student edit mode. 
    /// </summary>
	private void StartEditMode()
	{
#if !DEBUG
        if (_isInEditMode)
        {
            throw new InvalidOperationException("Edit mode is already active.");
        }
#endif

        AutoMapper.Map(Student, _editStudent);
        _isInEditMode = true;
        StateHasChanged();
    }    

#endregion
}
