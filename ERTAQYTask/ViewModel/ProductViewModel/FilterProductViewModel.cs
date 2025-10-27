using System.ComponentModel.DataAnnotations;

namespace PLProject.ViewModel.ProductViewModel
{
    public class FilterProductViewModel
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

        public List<GetAllProductViewModel> Products { get; set; } = new List<GetAllProductViewModel>();
        public List<PLProject.ViewModel.ServiceProviderViewModel.GetAllServiceProviderViewModel> ServiceProviders { get; set; } = new List<PLProject.ViewModel.ServiceProviderViewModel.GetAllServiceProviderViewModel>();
    }
}
