using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Project.Data;
using Project.Models;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Metadata;
using WebApplication6.Service;

namespace Project.Controllers
{
    public class BlogController : Controller
    {
        private readonly ShopContext _context;
        private readonly ICloudinaryService _cloudinaryService;


        public BlogController(ShopContext context, ICloudinaryService temp)
        {
            _context = context;
            _cloudinaryService = temp;

        }

        public IActionResult Index()
        {
            var blogs = _context.Blogs
            .Include(b => b.ImageBlogs)
            .Where(b => b.ImageBlogs.Any(ib => ib.IsBigImg == true))
            .Select(b => new
            {
                b.name,
                b.Blogid,
                b.HomeStatus,
                b.content,
                b.DateUp,
                FirstImage = b.ImageBlogs.Single(ib => ib.IsBigImg == true).ImageURL
            })
            .ToList();



            return View(blogs);
        }


        public IActionResult BlogDetails(int blogId)
        {
            var check = _context.Blogs.Find(blogId);

            if (check == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var blog = _context.Blogs
               .Include(b => b.Products)
               .Include(b => b.ImageBlogs)
               .Include(c => c.Comment)
                .ThenInclude(c => c.User)
               .Include(c => c.Comment)
                .ThenInclude(d => d.Answers)
                .ThenInclude(c => c.User)

               .FirstOrDefault(b => b.Blogid == blogId);
                return View(blog);
            }
        }

        [Authorize(Roles = "Seller, Admin, Marketing")]
        public IActionResult ViewBlog()
        {
            List<Blog> blog = _context.Blogs.ToList();


            return View(blog);
        }

        public IActionResult ChangeStatus(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Blogid == id);

            if (blog != null)
            {
                blog.HomeStatus = !blog.HomeStatus;
                _context.SaveChanges();
            }

            return RedirectToAction("ViewBlog");
        }

        public IActionResult Delete(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Blogid == id);

            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                _context.SaveChanges();
            }

            return RedirectToAction("ViewBlog");
        }

        public IActionResult ViewDetail(int id)
        {
            Blog blog = _context.Blogs.Include(b => b.ImageBlogs)
                                      .Include(b => b.Products)
                                      .FirstOrDefault(b => b.Blogid == id);

            if (blog == null)
            {
                TempData["checked"] = "";
                return Redirect("/Blog/ViewBlog");
            }

            return View(blog);
        }

        [Authorize(Roles = "Seller, Admin, Marketing")]
        public IActionResult CreateBlog()
        {
            return View();
        }

        [Authorize(Roles = "Seller, Admin, Marketing")]
        [HttpPost]
        public IActionResult CreateBlog(Blog blog)
        {
            _context.Blogs.Add(blog);
            _context.SaveChanges();

            return Redirect($"CreateImage?id={blog.Blogid}");

        }

        public IActionResult CreateImage(int id)
        {

            var blog = _context.Blogs.Include(b => b.ImageBlogs).FirstOrDefault(b => b.Blogid == id);


            return View(blog);
        }

        [HttpPost]
        public IActionResult CreateImage(IFormFile ImageUrl, ImageBlog imageBlog)
        {
            var UrlImage = _cloudinaryService.UploadImage(ImageUrl, "ImageBlog");

            imageBlog.ImageURL = UrlImage;

            _context.ImageBlogs.Add(imageBlog);
            _context.SaveChanges();

            return Redirect($"CreateImage?id={imageBlog.BlogId}");

        }

        public IActionResult Update(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Blogid == id);

            if (blog != null)
            {
                return View(blog);
            }
            else
            {
                TempData["checked"] = "";
                return Redirect("/Blog/ViewBlog");
            }
        }

        [HttpPost]
        public IActionResult Update(Blog updatedBlog)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Blogid == updatedBlog.Blogid);

            if (blog != null)
            {
                blog.HomeStatus = updatedBlog.HomeStatus;
                blog.content = updatedBlog.content;
                blog.DateUp = updatedBlog.DateUp;
                blog.name = updatedBlog.name;
                blog.isCollection = updatedBlog.isCollection;
                blog.content2 = updatedBlog.content2;

                _context.SaveChanges();
            }

            return Redirect($"UpdateImage?id={updatedBlog.Blogid}");
        }

        public IActionResult UpdateImage(int id)
        {
            List<ImageBlog> img = _context.ImageBlogs
                .Where(image => image.BlogId == id)
                .ToList();
            var blog = _context.Blogs.FirstOrDefault(b => b.Blogid == id);

            if (blog == null)
            {
                TempData["checked"] = "";
                return Redirect("/Blog/ViewBlog");
            }

            return View(img);
        }

        [HttpPost]
        public IActionResult CreateImage2(IFormFile ImageUrl, ImageBlog imageBlog)
        {
            var UrlImage = _cloudinaryService.UploadImage(ImageUrl, "ImageBlog");

            imageBlog.ImageURL = UrlImage;

            _context.ImageBlogs.Add(imageBlog);
            _context.SaveChanges();

            return Redirect($"UpdateImage?id={imageBlog.BlogId}");

        }

        [HttpPost]
        public async Task<IActionResult> UpdateImage(int ImageId, int BlogId, string ImageUrl, bool IsBigImg, DateTime DateUp, [FromForm] IFormFile NewImageUrl, int FormId)
        {
            if (FormId == ImageId)
            {

                var newImageUrl = ImageUrl;

                if (NewImageUrl?.Length > 0)
                {
                    newImageUrl = _cloudinaryService.UploadImage(NewImageUrl, "ImageBlog");
                }

                var image = _context.ImageBlogs.FirstOrDefault(i => i.ImageBlogId == ImageId);

                Console.WriteLine(newImageUrl);

                if (image != null)
                {
                    image.BlogId = BlogId;

                    image.ImageURL = newImageUrl;

                    image.IsBigImg = IsBigImg;
                    image.DateUp = DateUp;

                    _context.ImageBlogs.Update(image);
                    _context.SaveChanges();
                    return RedirectToAction("UpdateImage", new { id = BlogId });
                }
            }

            return RedirectToAction("ViewBlog");
        }

        public IActionResult searchBlogByName(string name)
        {
            List<Blog> blog = null;

            if (name != null)
            {
                blog = _context.Blogs.Where(x => x.name.Contains(name)).ToList();
            }
            else

                blog = _context.Blogs.ToList();

            return View(blog);
        }






    }

}

