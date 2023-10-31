namespace Project.Models
{
    public class WishList
    {
        public int WishListId { get; set; }
        public string UserId { get; set; } = null!;
        public int ProductId { get; set; }

    }
}
