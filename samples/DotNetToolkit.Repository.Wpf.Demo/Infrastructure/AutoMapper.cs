namespace DotNetToolkit.Repository.Wpf.Demo.Infrastructure
{
    using System;
    using Interfaces;

    public static class AutoMapper
    {
        public static void Map<TSource, TTarget>(TSource source, TTarget target)
            where TSource : ICustomer
            where TTarget : ICustomer
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.Id = source.Id;
            target.Name = source.Name;
            target.Notes = source.Notes;
            target.Date = source.Date;
        }

        public static TTarget Map<TSource, TTarget>(TSource source)
            where TSource : ICustomer
            where TTarget : ICustomer, new()
        {
            var target = new TTarget();

            Map(source, target);

            return target;
        }
    }
}
