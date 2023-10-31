using Microsoft.AspNetCore.Identity;

namespace Project.Models
{
    public class Answer
    {
        public int answerId { get; set; }
        public string UserId { get; set; } = null!;
        public string content { get; set; } = null!;
        public int commentId { get; set; }

        public Comment Comment { get; set; }
        public IdentityUser User { get; set; }

    }
}
