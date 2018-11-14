namespace DotNetToolkit.Repository.InMemory.Internal
{
    /// <summary>
    /// Represents an internal state for an entity in the in-memory store.
    /// </summary>
    internal enum EntityState
    {
        Added,
        Removed,
        Modified
    }
}
