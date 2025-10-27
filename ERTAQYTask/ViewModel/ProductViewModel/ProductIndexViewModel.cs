using PLProject.ViewModel.ProductViewModel;

namespace PLProject.ViewModel.ProductViewModel
{
    public class ProductIndexViewModel
    {
        public IEnumerable<GetAllProductViewModel> Products { get; set; }
        public FilterProductViewModel Filter { get; set; }
    }
}
