namespace DotNetToolkit.Repository.Wpf.Demo.Models
{
    using Infrastructure.Interfaces;
    using System;

    public class Customer : ICustomer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public DateTime? Date { get; set; }
    }
}
