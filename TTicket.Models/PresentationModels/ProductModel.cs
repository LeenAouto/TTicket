namespace TTicket.Models.PresentationModels
{
    public class ProductModel
    {
        public ProductModel() { }
        public ProductModel(Product product)
        {
            Id = product.Id;
            Name = product.Name;
        }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
