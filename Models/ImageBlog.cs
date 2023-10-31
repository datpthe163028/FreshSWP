namespace Project.Models
{
	public class ImageBlog
	{
		public int ImageBlogId { get; set; }
		public int BlogId { get; set; }
		public string ImageURL { get; set; } = null!;
		public bool IsBigImg { get; set; }

		public DateTime? DateUp { get; set; }
		public virtual Blog Blog { get; set; } = null!;
	}
}
