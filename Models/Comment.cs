using Microsoft.AspNetCore.Identity;

namespace Project.Models
{
    public class Comment
    {
        public int commentId {  get; set; }
        public string UserId { get; set; } = null!;
        public int  Blogid { get; set; }
        public string Content { get; set; } = null!;

        public List<Answer> Answers { get; set; }

        public IdentityUser User { get; set; }

        public Blog Blog { get; set; }


    }
}
