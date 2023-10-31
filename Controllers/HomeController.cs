using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Diagnostics;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShopContext _shopContext;
        private readonly SignInManager<IdentityUser> signInManager;

        public HomeController(ILogger<HomeController> logger, ShopContext ct, SignInManager<IdentityUser> _signInManager)
        {
            _shopContext = ct;
            _logger = logger;
            signInManager = _signInManager;
        }

        public IActionResult Index(string mode)
        {

            var listProduct = _shopContext.Products.Where(p => p.HomeStatus == true);
            if (mode != null)
            {
                if (mode.Equals("EDetailProduct"))
                {
                    ViewData["EDetailProduct"] = "Error";
                }
            }

            ViewData["listBlog"] = _shopContext.Blogs.Include(p => p.ImageBlogs).Where(p => p.HomeStatus == true).OrderBy(p => p.Blogid).ToList();
            return View(listProduct.ToList());


        }
        public IActionResult Address()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Address(Address a, string phone)
        {

            if (!int.TryParse(phone, out int number) || phone.Length < 9 || phone.Length > 11)
            {
                ViewData["Error"] = "Không đúng định dạng";
                return View();
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!signInManager.IsSignedIn(User))
            {
                return View();
            }
            else
            {
                _shopContext.Users.Find(signInManager.UserManager.GetUserId(User)).PhoneNumber = phone;
                _shopContext.Addresses.Add(new Address() { UserId = signInManager.UserManager.GetUserId(User), District = a.District, Province = a.Province, Town = a.Town, SpecificAdd = a.SpecificAdd });
                _shopContext.SaveChanges();
                return View();
            }
        }

        public IActionResult Remove(int id)
        {
            var x = _shopContext.Addresses.Find(id);

            if (x == null)
            {
                return Redirect("/Home/Address");
            }
            else
            {
                _shopContext.Addresses.Remove(x);
                _shopContext.SaveChanges();
                return Redirect("/Home/Address");
            }
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}