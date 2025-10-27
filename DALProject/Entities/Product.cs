using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DALProject.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public int ServiceProviderId { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
    }
}