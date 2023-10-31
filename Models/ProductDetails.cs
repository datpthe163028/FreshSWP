namespace Project.Models
{
    public class ProductDetails
    {
        public int productDetailId { get; set; }
        public int quantity { get; set; }
        public string color { get; set; } = null!;
        public string size { get; set; } = null!;

        public int productId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
