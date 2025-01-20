using AzureStudents.Api.Controllers;
using AzureStudents.Api.Services;
using AzureStudents.Shared.Constants;
using AzureStudents.Shared.Dto.Authentication;
using Moq;
using System.Security.Claims;

namespace AzureStudents.Test.Tests.Controllers;

/// <summary>
/// Provides tests for the authentication controller. 
/// </summary>
public class AuthenticationControllerTest : ControllerTestBase
{
    #region HelperMethods

    /// <summary>
    /// Creates an authentication controller with a mocked context.
    /// </summary>
    /// <returns>A <see cref="Task"/> containing a <see cref="AuthenticationController"/>.</returns>
    private AuthenticationController CreateAuthenticationController()
    {
        // User
        ClaimsPrincipal claimsPrincipal = CreateClaimsPrincipalMock(isUserAuthenticated: false).Object;

        // Controller context
        var controllerContextMock = CreateControllerContextMock(claimsPrincipal);

        // Token service 
        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.CreateToken())
            .Returns("ThisIsAVeryLongMockTokenThatLoopsThisIsAVeryLongMockTokenThatLoopsThisIsAVeryLongMockTokenThatLoops");

        // Authentication controller
        var authenticationController = new AuthenticationController(tokenServiceMock.Object);
        authenticationController.ControllerContext = controllerContextMock.Object;

        return authenticationController;
    }

    #endregion

    #region Tests

    /// <summary>
    /// Tests logging in the user.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [Fact]
    public void LoginUser_ShouldReturnJwtToken()
    {
        // Arrange
        var controller = CreateAuthenticationController();
        LoginDto loginDto = new LoginDto(ApplicationConstants.FrontEndApplicationId);

        // Act
        var loginResponse = controller.Login(loginDto);

        // Assert
        AssertOkResponse<LoginResponseDto>(loginResponse, loginData =>
        {
            Assert.False(string.IsNullOrEmpty(loginData.Token));
        });
    }

    #endregion
}
