using Microsoft.AspNetCore.Components;

namespace AzureStudents.Blazor.Pages;

/// <summary>
/// Component for the home page.
/// </summary>
public partial class Home : ComponentBase
{
    #region Fields

    /// <summary>
    /// True if the component have been initialized. 
    /// </summary>
    private bool _isInitialized;

    #endregion

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
#if DEBUG
        // Let the backend container start first. 
        await Task.Delay(500);
#endif

        await base.OnInitializedAsync();
        _isInitialized = true;
    }
}
