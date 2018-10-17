namespace DotNetToolkit.Repository.Internal
{
    using Configuration;
    using Configuration.Options;
    using System;

    internal class RepositoryOptionsContextExtension : IRepositoryOptionsExtensions
    {
        public RepositoryOptionsContextExtension(IRepositoryContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Context = context;
        }

        public IRepositoryContext Context { get; }
    }
}
