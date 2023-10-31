namespace Project.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string size { get; set; } = null!;
        public string color { get; set; } = null!;
        public Cart cart { get; set; } = null!;
        public Product product { get; set; } = null!;
    }
}
