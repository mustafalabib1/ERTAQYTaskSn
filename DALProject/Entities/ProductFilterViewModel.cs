using System.ComponentModel.DataAnnotations;

namespace DALProject.Entities
{
    public class ProductFilterViewModel
    {
        [Display(Name = "Minimum Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum price must be positive")]
        public decimal? MinPrice { get; set; }

        [Display(Name = "Maximum Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Maximum price must be positive")]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Service Provider")]
        public int? ServiceProviderId { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
        public List<ServiceProvider> ServiceProviders { get; set; } = new List<ServiceProvider>();
    }
}