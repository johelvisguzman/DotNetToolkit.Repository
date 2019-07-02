namespace DotNetToolkit.Repository.NHibernate
{
    using Configuration;
    using global::NHibernate;

    /// <summary>
    /// Represents a nhibernate repository context.
    /// </summary>
    /// <seealso cref="IRepositoryContextAsync" />
    public interface INHibernateRepositoryContext : IRepositoryContextAsync
    {
        /// <summary>
        /// Gets the current session.
        /// </summary>
        ISession Session { get; }
    }
}
