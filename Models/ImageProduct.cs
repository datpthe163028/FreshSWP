namespace Project.Models
{
    public class ImageProduct
    {
        public int ImageProductId { get; set; }
        public int ProductId { get; set; }
        public string ImageURL { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
