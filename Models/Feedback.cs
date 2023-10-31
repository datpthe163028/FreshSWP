using Microsoft.AspNetCore.Identity;

namespace Project.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime FeedbackDate { get; set; }
        public string FeedbackTitle { get; set; } = null!;
        public string FeedbackDetail { get; set; } = null!;
        public string FeedbackAnswer { get; set; } = null!;
        public string FeedbackStatus { get; set; } = null!;
        public int ProductId { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual IdentityUser User { get; set; } = null!;
    }
}
