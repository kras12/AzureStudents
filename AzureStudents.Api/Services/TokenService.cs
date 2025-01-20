using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using AzureStudents.Api.Constants;
using AzureStudents.Shared.Constants;
using AzureStudents.Api.Configuration;
using Microsoft.Extensions.Options;

namespace AzureStudents.Api.Services;

/// <summary>
/// Creates JWT tokens. 
/// </summary>
public class TokenService : ITokenService
{
    #region Constants

    /// <summary>
    /// The duration of the token in days.
    /// </summary>
    public const int TokenDurationInDays = 3;

    #endregion

    #region Fields

    /// <summary>
    /// The injected JWT settings.
    /// </summary>
    private readonly JwtSettings _jwtSettings;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="jwtSettings">The injected JWT settings.</param>
    public TokenService(IOptions<JwtSettings> jwtSettings)
    { 
        _jwtSettings = jwtSettings.Value;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a token.
    /// </summary>
    /// <returns>A token in the form of a <see cref="string"/>.</returns>
    public string CreateToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        var claims = new List<Claim>
        {
            new Claim(ApplicationUserJwtClaims.ApplicationId, ApplicationConstants.FrontEndApplicationId)
        };
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(TokenDurationInDays),
            SigningCredentials = credentials,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var tokenHandler = new JsonWebTokenHandler();
        tokenHandler.SetDefaultTimesOnTokenCreation = false;

        return tokenHandler.CreateToken(tokenDescriptor);
    }

    #endregion
}
