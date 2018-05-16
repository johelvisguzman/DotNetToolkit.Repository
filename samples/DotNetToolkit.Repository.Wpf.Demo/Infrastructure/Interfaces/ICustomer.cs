namespace DotNetToolkit.Repository.Wpf.Demo.Infrastructure.Interfaces
{
    using System;

    public interface ICustomer
    {
        int Id { get; set; }
        string Name { get; set; }
        string Notes { get; set; }
        DateTime? Date { get; set; }
    }
}