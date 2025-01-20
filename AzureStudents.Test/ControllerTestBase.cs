using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Moq;
using AzureStudents.Test.Tests;
using AzureStudents.Api.Constants;
using AzureStudents.Shared.Constants;
using AutoMapper;
using AzureStudents.Api.Mapping;
using AzureStudents.Shared.Dto.Api;
using AzureStudents.Shared.Enums;

namespace AzureStudents.Test;

/// <summary>
/// Base class for MVC controller tests. 
/// </summary>
public class ControllerTestBase : TestBase
{
    #region Fields

    /// <summary>
    /// The AutoMapper instance. 
    /// </summary>
    protected readonly IMapper _autoMapper;

    #endregion

    #region Constructors

    public ControllerTestBase()
    {
        MapperConfiguration config = new MapperConfiguration(config =>
        {
            config.AddProfile(new AutoMapperProfile());
        });

        _autoMapper = new Mapper(config);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Performs assertions for an OK response.
    /// </summary>
    /// <typeparam name="T">The value type of the API response.</typeparam>
    /// <param name="controllerResponse">The controller response.</param>
    /// <param name="assertActions">Assert actions to perform on the value of the API response.</param>
    /// <param name="expectsNullValue">Set to true if the API response value is expected to be null.</param>
    protected void AssertOkResponse<T>(IActionResult? controllerResponse, Action<T>? assertActions = null, bool expectsNullResponseValue = false) where T : class
    {
        Assert.NotNull(controllerResponse);
        Assert.IsType<OkObjectResult>(controllerResponse);

        var apiResponse = ((OkObjectResult)controllerResponse).Value as ApiResponseDto<T>;
        Assert.IsType<ApiResponseDto<T>>(apiResponse);
        Assert.True(apiResponse.Success);

        if (!expectsNullResponseValue)
        {
            Assert.NotNull(apiResponse.Value);
            assertActions?.Invoke(apiResponse.Value);
        }
    }

    /// <summary>
    /// Performs assertions for an unauthorized response. 
    /// </summary>
    /// <typeparam name="T">The value type of the API response.</typeparam>
    /// <param name="controllerResponse">The controller response.</param>
    protected void AssertUnauthorizedResponse<T>(IActionResult? controllerResponse) where T : class
    {
        Assert.NotNull(controllerResponse);
        Assert.IsType<UnauthorizedObjectResult>(controllerResponse);

        var apiResponse = ((UnauthorizedObjectResult)controllerResponse).Value as ApiResponseDto<T>;
        Assert.IsType<ApiResponseDto<T>>(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains(apiResponse.Errors, x => x.ErrorType == ApiErrorMessageTypes.AuthorizationError.ToString());
    }

    /// <summary>
    /// Creates a mocked claims principal.
    /// </summary>
    /// <param name="isUserAuthenticated">True if the user is authenticated.</param>
    /// <returns>A mocked <see cref="ClaimsPrincipal"/>.</returns>
    protected Mock<ClaimsPrincipal> CreateClaimsPrincipalMock(bool isUserAuthenticated)
    {
        var result = new Mock<ClaimsPrincipal>();
        result.SetupGet(x => x.Identity!.IsAuthenticated).Returns(isUserAuthenticated);
        result.Setup(x => x.AddIdentity(It.IsAny<ClaimsIdentity>())).CallBase();
        result.CallBase = true;

        if (isUserAuthenticated)
        {
            result.Object.AddIdentity(new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ApplicationUserJwtClaims.ApplicationId, ApplicationConstants.FrontEndApplicationId)
                }));
        }

        return result;
    }

    /// <summary>
    /// Creates a mocked controller context. 
    /// </summary>
    /// <param name="user"></param>
    /// <returns>A mocked <see cref="ControllerContext"/> object.</returns>
    protected Mock<ControllerContext> CreateControllerContextMock(ClaimsPrincipal user)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = user;
        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(),
            new ControllerActionDescriptor(), new ModelStateDictionary());

        return new Mock<ControllerContext>(actionContext);
    }
    #endregion
}
