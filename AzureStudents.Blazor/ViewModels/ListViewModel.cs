namespace AzureStudents.Blazor.ViewModels;

/// <summary>
/// A view model that contains a list of other view models.
/// </summary>
/// <typeparam name="T">A class based on the <see cref="ViewModelBase"/> class.</typeparam>
public class ListViewModel<T> where T : class
{
    #region Constructors

    /// <summary>
    /// A constructor.
    /// </summary>
    public ListViewModel()
    {

    }

    /// <summary>
    /// A constructor.
    /// </summary>
    /// <param name="models">A collection of view models to add.</param>
    public ListViewModel(IEnumerable<T> models)
    {
        Models = new List<T>(models);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns the number of models in the collection.
    /// </summary>
    public int Count
    {
        get
        {
            return Models.Count;
        }
    }

    /// <summary>
    /// Returns true if there are any models in the collection. 
    /// </summary>
    public bool HaveModels
    {
        get
        {
            return Models.Count > 0;
        }
    }

    /// <summary>
    /// A list of view models. 
    /// </summary>
    public List<T> Models { get; set; } = new();

    #endregion
}
