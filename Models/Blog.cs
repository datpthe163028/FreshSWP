namespace Project.Models
{
    public class Blog
    {
        public int Blogid { get; set; }
        public bool? HomeStatus { get; set; }
        public string content { get; set; } = null!;
        public string content2 { get; set; } = null!;
        public DateTime? DateUp { get; set; }
        public string name { get; set; } = null!;
        public bool isCollection { get; set; }
        public virtual List<ImageBlog> ImageBlogs { get; set; } = null!;
        public virtual List<Product> Products { get; set; } = null!;
        public virtual List<Comment> Comment { get; set; }=null!;

    }
}
