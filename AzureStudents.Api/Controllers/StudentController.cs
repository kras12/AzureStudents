﻿using AutoMapper;
using AzureStudents.Api.Constants;
using AzureStudents.Api.Services;
using AzureStudents.Data.Entities;
using AzureStudents.Data.Repositories;
using AzureStudents.Shared.Dto.Api;
using AzureStudents.Shared.Dto.Student;
using AzureStudents.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AzureStudents.Api.Controllers;

/// <summary>
/// Handles student related activities. 
/// </summary>
[Route("students-api/students")]
[ApiController]
public class StudentController : ControllerBase
{
    #region Fields

    /// <summary>
    /// The injected user authorization service.
    /// </summary>
    protected readonly IUserAuthorizationService _userAuthorizationService;

    // The injected Auto Mapper.
    private readonly IMapper _mapper;

    /// <summary>
    /// The injected student repository.
    /// </summary>
    private readonly IStudentRepository _studentRepository;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="studentRepository">The injected student repository.</param>
    /// <param name="mapper">The injected Auto Mapper.</param>
    /// <param name="userAuthorizationService">The injected user authorization service.</param>
    public StudentController(IStudentRepository studentRepository, IMapper mapper, IUserAuthorizationService userAuthorizationService)
    {
        _studentRepository = studentRepository;
        _mapper = mapper;
        _userAuthorizationService = userAuthorizationService;
    }

    #endregion

    #region Endpoints        

    /// <summary>
    /// Creates a new student.
    /// </summary>
    /// <param name="studentDto">The input data for the new student.</param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing the result of the operation.</returns>
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentDto studentDto)
    {
        try
        {
            if (!await _userAuthorizationService.IsUserAuthorized(User, ApplicationUserPolicies.FrontEndApplicationPolicy))
            {
                return Unauthorized(CreateUnauthorizedResponse());
            }

            var student = _mapper.Map<Student>(studentDto);
            await _studentRepository.AddAsync(student);

            return Ok(ApiResponseDto<StudentDto>.CreateSuccessfulResponse(_mapper.Map<StudentDto>(student)));
        }
        catch (Exception ex)
        {
            return CreateGeneralErrorResponse();
        }
    }

    /// <summary>
    /// Deletes a student
    /// </summary>
    /// <param name="id">The ID for the student.</param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing the result of the operation.</returns>
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status404NotFound)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (!await _userAuthorizationService.IsUserAuthorized(User, ApplicationUserPolicies.FrontEndApplicationPolicy))
            {
                return Unauthorized(CreateUnauthorizedResponse());
            }

            if (id <= 0)
            {
                return BadRequest(ApiResponseDto<StudentDto>.CreateErrorResponse(ApiErrorMessageTypes.InvalidInputData, $"Invalid student id: {id}"));
            }

            if (!await _studentRepository.StudentExists(id))
            {
                return NotFound(ApiResponseDto<StudentDto>.CreateErrorResponse(ApiErrorMessageTypes.ResourceNotFound, $"Failed to find a student with ID: {id}"));
            }

            await _studentRepository.DeleteAsync(id);

            return Ok(ApiResponseDto<StudentDto>.CreateSuccessfulResponse());
        }
        catch (Exception ex)
        {
            return CreateGeneralErrorResponse();
        }
    }

    /// <summary>
    /// Edits a student.
    /// </summary>
    /// <param name="studentDto">The edited data for the student.</param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing the result of the operation.</returns>
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status404NotFound)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, UpdateStudentDto studentDto)
    {
        try
        {
            if (!await _userAuthorizationService.IsUserAuthorized(User, ApplicationUserPolicies.FrontEndApplicationPolicy))
            {
                return Unauthorized(CreateUnauthorizedResponse());
            }

            if (id <= 0)
            {
                return BadRequest(ApiResponseDto<StudentDto>.CreateErrorResponse(ApiErrorMessageTypes.InvalidInputData, $"Invalid student id: {id}"));
            }

            var student = await _studentRepository.GetByIdAsync(id);

            if (student == null)
            {
                return NotFound(ApiResponseDto<StudentDto>.CreateErrorResponse(ApiErrorMessageTypes.ResourceNotFound, $"Failed to find a student with ID: {id}"));
            }

            _mapper.Map(studentDto, student);
            await _studentRepository.UpdateAsync(student);

            return Ok(ApiResponseDto<StudentDto>.CreateSuccessfulResponse(_mapper.Map<StudentDto>(student)));
        }
        catch (Exception ex)
        {
            return CreateGeneralErrorResponse();
        }
    }

    /// <summary>
    /// Gets a student by ID.
    /// </summary>
    /// <param name="id">The ID for the student.</param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing the result of the operation.</returns>
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status404NotFound)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetStudentById(int id)
    {
        try
        {
            if (!await _userAuthorizationService.IsUserAuthorized(User, ApplicationUserPolicies.FrontEndApplicationPolicy))
            {
                return Unauthorized(CreateUnauthorizedResponse());
            }

            if (id <= 0)
            {
                return BadRequest(ApiResponseDto<StudentDto>.CreateErrorResponse(ApiErrorMessageTypes.InvalidInputData, $"Invalid student id: {id}"));
            }

            var student = await _studentRepository.GetByIdAsync(id);

            if (student == null)
            {
                return NotFound(ApiResponseDto<StudentDto>.CreateErrorResponse(ApiErrorMessageTypes.ResourceNotFound, $"Failed to find a student with ID: {id}"));
            }

            return Ok(ApiResponseDto<StudentDto>.CreateSuccessfulResponse(_mapper.Map<StudentDto>(student)));
        }
        catch (Exception ex)
        {
            return CreateGeneralErrorResponse();
        }
    }

    /// <summary>
    /// Gets all students.
    /// </summary>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing the result of the operation.</returns>
    [ProducesResponseType<ApiResponseDto<List<StudentDto>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponseDto<StudentDto>>(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    public async Task<IActionResult> List()
    {
        try
        {
            if (!await _userAuthorizationService.IsUserAuthorized(User, ApplicationUserPolicies.FrontEndApplicationPolicy))
            {
                return Unauthorized(CreateUnauthorizedResponse());
            }

            var students = _mapper.Map<List<StudentDto>>((await _studentRepository.GetAll()).ToList());
            return Ok(ApiResponseDto<List<StudentDto>>.CreateSuccessfulResponse(students));
        }
        catch (Exception ex)
        {
            return CreateGeneralErrorResponse();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a response object for unathorized users.
    /// </summary>
    /// <param name="message">An optional custom message to use.</param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> object.</returns>
    protected ApiResponseDto<StudentDto> CreateUnauthorizedResponse(string? message = null)
    {
        return ApiResponseDto<StudentDto>.CreateErrorResponse(ApiErrorMessageTypes.AuthorizationError, message ?? "Authorization failed.");
    }

    /// <summary>
    /// Returns a standard response for general errors.
    /// </summary>
    /// <returns><see cref="ObjectResult"/>.</returns>
    private ObjectResult CreateGeneralErrorResponse()
    {
        return StatusCode(500, ApiResponseDto<StudentDto>.CreateErrorResponse(ApiErrorMessageTypes.GeneralError, "An unexpected error occurred."));
    }

    #endregion
}
