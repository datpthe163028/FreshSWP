using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.ComponentModel.Design;

namespace Project.Controllers
{
    public class CommentController : Controller
    {
        private readonly ShopContext _shopContext;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentController(ShopContext shopContext, UserManager<IdentityUser> userManager)
        {
            _shopContext = shopContext;
            _userManager = userManager;

        }
        public IActionResult AddComment(IFormCollection form)
        {
            if (ModelState.IsValid)
            {

                var userId = _userManager.GetUserId(HttpContext.User);
                if (userId == null)
                {
                    return Redirect("~/Identity/Account/Login");
                }
                var blogId = int.Parse(form["blogId"]);
                var content = form["content"];

                var comment = new Comment
                {
                    UserId = userId,
                    Blogid = blogId,
                    Content = content
                };
                _shopContext.Comment.Add(comment);
                _shopContext.SaveChanges();

                return RedirectToAction("BlogDetails", "Blog", new { blogId = blogId });


            }
            else
            {
                return NotFound();

            }
        }

        public IActionResult AddAnswer(IFormCollection form)
        {
            if (ModelState.IsValid)
            {

                var userId = _userManager.GetUserId(HttpContext.User);
                if (userId == null)
                {
                    return Redirect("~/Identity/Account/Login");
                }
                var commentId = int.Parse(form["commentId"]);
                var content = form["content"];
                var blogId = int.Parse(form["blogId"]);

                var answer = new Answer
                {
                    UserId = userId,
                    commentId = commentId,
                    content = content
                };
                _shopContext.Add(answer);
                _shopContext.SaveChanges();

                return RedirectToAction("BlogDetails", "Blog", new { blogId = blogId });


            }
            else
            {
                return NotFound();

            }
        }

        public IActionResult deleteComment(string commentId, string blogId)
        {
            var Comment = _shopContext.Comment.FirstOrDefault(x => x.commentId == int.Parse(commentId));

            if (blogId == null || Comment == null)
            {
                return NotFound();
            }
            var answers = _shopContext.Answer.Where(a => a.commentId == int.Parse(commentId));
            _shopContext.Answer.RemoveRange(answers);
            _shopContext.Comment.Remove(Comment);
            _shopContext.SaveChanges();
            var blogID = int.Parse(blogId);
            return RedirectToAction("BlogDetails", "Blog", new { blogId = blogID });

        }


        public IActionResult deleteAnswer(string answerId, string blogId)
        {
            var Answer = _shopContext.Answer.FirstOrDefault(x => x.answerId == int.Parse(answerId));

            if (blogId == null || Answer == null)
            {
                return NotFound();
            }
            _shopContext.Answer.Remove(Answer);
            _shopContext.SaveChanges();

            var blogID = int.Parse(blogId);
            return RedirectToAction("BlogDetails", "Blog", new { blogId = blogID });
        }
    }
}
