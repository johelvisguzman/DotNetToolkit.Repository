namespace DotNetToolkit.Repository.Performance.Data
{
    using System.ComponentModel.DataAnnotations;

    public class Customer
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
