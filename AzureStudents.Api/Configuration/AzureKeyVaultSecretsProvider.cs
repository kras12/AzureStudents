using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace AzureStudents.Api.Configuration;

/// <summary>
/// Provider used for fetching secrets from Azure Key Vault. 
/// </summary>
public class AzureKeyVaultSecretsProvider
{
    #region Fields

    /// <summary>
    /// The URI for the key vault.
    /// </summary>
	private readonly Uri _keyVaultUri;

    /// <summary>
    /// An optional logger.
    /// </summary>
    private readonly ILogger? _logger;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor. 
    /// </summary>
    /// <param name="keyVaultUri">The URI for the key vault.</param>
    /// <param name="logger">An optional logger.</param>
    public AzureKeyVaultSecretsProvider(Uri keyVaultUri, ILogger? logger = null)
    {
        _keyVaultUri = keyVaultUri;
        _logger = logger;


        if (_logger != null)
        {
            _logger?.LogInformation($"Creating an instance of class '{nameof(AzureKeyVaultSecretsProvider)}'.");
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Retrieves secrets from the vault. 
    /// </summary>
    /// <param name="secretNames">A collection of secrets to fetch.</param>
    /// <returns><see cref="Dictionary{TKey, TValue}"/> where <c>TKey</c> is <see cref="string"/> and <c>TValue</c> is <see cref="string?"/></returns>
    public Dictionary<string, string?> GetSecrets(List<string> secretNames)
    {
        Dictionary<string, string?> result = new();

        _logger?.LogInformation($"Start fetching {secretNames.Count} Azure Key Vault secrets.");

        var secretClient = new SecretClient(_keyVaultUri, new DefaultAzureCredential());

        foreach (var secretName in secretNames)
        {
            _logger?.LogInformation($"Fetching secret from Azure Key Vault: '{secretName}'");

            var configNameParts = secretName.Split("--");
            string finalConfigName = string.Join(":", configNameParts);
            var secret = secretClient.GetSecret(secretName);

            if (!string.IsNullOrEmpty(secret.Value.Value))
            {
                _logger?.LogInformation($"Successfully fetched secret: '{secretName}'");
                _logger?.LogInformation($"Adding secret under config name: '{finalConfigName}'.");
            }
            else
            {
                _logger?.LogError($"No value found for secret: '{secretName}'");
            }

            result.Add(finalConfigName, secret.Value.Value);
        }

        return result;
    }

    #endregion
}
