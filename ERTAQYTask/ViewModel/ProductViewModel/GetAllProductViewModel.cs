namespace PLProject.ViewModel.ProductViewModel
{
    public class GetAllProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreationDate { get; set; }
        public int ServiceProviderId { get; set; }
        public string? ServiceProviderName { get; set; }
    }
}