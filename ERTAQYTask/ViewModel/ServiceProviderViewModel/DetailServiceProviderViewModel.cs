using PLProject.ViewModel.ProductViewModel;
using System.ComponentModel.DataAnnotations;

namespace PLProject.ViewModel.ServiceProviderViewModel
{
    public class DetailServiceProviderViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Service Provider Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        public IEnumerable<GetAllProductViewModel> Products{ get; set; } = new List<GetAllProductViewModel>();
    }
}
