namespace DotNetToolkit.Repository.Internal
{
    using Configuration.Options;
    using Factories;
    using System;

    internal class RepositoryOptionsContextFactoryExtension : IRepositoryOptionsContextFactoryExtensions
    {
        public RepositoryOptionsContextFactoryExtension(IRepositoryContextFactory contextFactory)
        {
            if (contextFactory == null)
                throw new ArgumentNullException(nameof(contextFactory));

            ContextFactory = contextFactory;
        }

        public IRepositoryContextFactory ContextFactory { get; }

        public Type ContextFactoryType { get { return ContextFactory.GetType(); } }
    }
}