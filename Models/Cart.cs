using Microsoft.AspNetCore.Identity;

namespace Project.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public string UserId { get; set; } = null!;

        public IdentityUser User { get; set; } = null!;
        public List<CartItem> CartItems { get; set; } = null!;
    }
}
