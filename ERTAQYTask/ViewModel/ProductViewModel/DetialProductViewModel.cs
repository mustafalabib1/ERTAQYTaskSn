using System.ComponentModel.DataAnnotations;

namespace PLProject.ViewModel.ProductViewModel
{
    public class DetialProductViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Product Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Creation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Service Provider ID")]
        public int ServiceProviderId { get; set; }

        [Display(Name = "Service Provider Name")]
        public string? ServiceProviderName { get; set; }
    }
}