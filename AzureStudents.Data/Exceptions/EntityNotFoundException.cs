namespace AzureStudents.Data.Exceptions;

/// <summary>
/// An exception that is thrown when an entity is not found.
/// </summary>
public class EntityNotFoundException : Exception
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">An optional message.</param>
    public EntityNotFoundException(string? message) : base(message)
    {
    }
}
