using AzureStudents.Api.Services;
using AzureStudents.Shared.Constants;
using AzureStudents.Shared.Dto.Api;
using AzureStudents.Shared.Dto.Authentication;
using AzureStudents.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AzureStudents.Api.Controllers;

/// <summary>
/// Handles authentication activities. 
/// </summary>
[Route("students-api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    #region Fields

    /// <summary>
    /// The injected token service. 
    /// </summary>
    private readonly ITokenService _tokenService;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor. 
    /// </summary>
    /// <param name="tokenService">The injected token service.</param>
    public AuthenticationController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    #endregion

    #region Endpoints

    /// <summary>
    /// Attempts to login.
    /// </summary>
    /// <param name="credentials">The credentials for the login.</param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> containing the result of the operation.</returns>
    [HttpPost("login")]
    [ProducesResponseType<ApiResponseDto<LoginResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponseDto<LoginResponseDto>>(StatusCodes.Status400BadRequest)]
    public IActionResult Login([FromBody] LoginDto credentials)
    {
        if (string.IsNullOrEmpty(credentials.ApplicationId))
        {
            return BadRequest(ApiResponseDto<LoginResponseDto>.CreateErrorResponse(ApiErrorMessageTypes.InvalidInputData, "Application ID is missing."));
        }

        if (credentials.ApplicationId != ApplicationConstants.FrontEndApplicationId)
        {
            return BadRequest(ApiResponseDto<LoginResponseDto>.CreateErrorResponse(ApiErrorMessageTypes.InvalidInputData, "Application ID is invalid."));
        }

        return Ok(ApiResponseDto<LoginResponseDto>.CreateSuccessfulResponse(new LoginResponseDto(credentials.ApplicationId, _tokenService.CreateToken())));
    }

    #endregion
}
