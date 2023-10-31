namespace Project.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;

        public DateTime ImportDate { get; set; }
        public string ProductDescription { get; set; } = null!;

        public double Discount { get; set; }
        public double ProductPrice { get; set; }
        public int? BlogId { get; set; }
        public bool IsAvailble { get; set; }
        public bool HomeStatus { get; set; }
        public int SubCategoryID { get; set; }
        public string ImageMain { get; set; } = null!;
        public virtual Blog Blog { get; set; } = null!;

        public virtual SubCategory SubCategory { get; set; } = null!;
        public virtual List<BillDetail> BillDetails { get; set; } = null!;
        public virtual List<Feedback> Feedbacks { get; set; } = null!;
        public virtual List<ImageProduct> ImageProducts { get; set; } = null!;
        public virtual List<ProductDetails> ProductDetails { get; set; } = null!;
        public virtual List<CartItem> CartItems { get; set; } = null!;

    }
}
