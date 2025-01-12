using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using AzureStudents.Api.Constants;
using AzureStudents.Shared.Constants;

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
    /// The injected configuration.
    /// </summary>
    private readonly IConfiguration _config;

    /// <summary>
    /// Represents a symmetric security key.
    /// </summary>
    private readonly SymmetricSecurityKey _key;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="config">The injected configuration manager.</param>
    public TokenService(IConfiguration config)
    {
        _config = config;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a token.
    /// </summary>
    /// <returns>A token in the form of a <see cref="string"/>.</returns>
    public string CreateToken()
    {
        var claims = new List<Claim>
        {
            new Claim(ApplicationUserJwtClaims.ApplicationId, ApplicationConstants.FrontEndApplicationId)
        };

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(TokenDurationInDays),
            SigningCredentials = creds,
            Issuer = _config["JWT:Issuer"],
            Audience = _config["JWT:Audience"]
        };

        var tokenHandler = new JsonWebTokenHandler();
        tokenHandler.SetDefaultTimesOnTokenCreation = false;

        return tokenHandler.CreateToken(tokenDescriptor);
    }

    #endregion
}
