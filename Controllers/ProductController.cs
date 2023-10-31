using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Project.Data;
using Project.Models;
using System.Collections.Generic;

namespace Project.Controllers
{
    public class ProductController : Controller
    {

        private readonly ILogger<ProductController> _logger;
        private readonly ShopContext _shopContext;
        private readonly SignInManager<IdentityUser> _signInManager;
        public ProductController(ILogger<ProductController> logger, ShopContext ct, SignInManager<IdentityUser> s1)
        {
            _shopContext = ct;
            _logger = logger;
            _signInManager = s1;
        }



        public IActionResult Index(int id, int mode)
        {

            List<SubCategory> subCategories = new List<SubCategory>();
            var list = (from p in _shopContext.Products
                        where p.SubCategoryID == id
                        select p).ToList();

            if(list == null)
            {
                TempData["subCate"] = "";
                return Redirect("/Home/Index");
            }
               

            var subCate = (from c in _shopContext.SubCategory
                           where c.SubCategoryId == id
                           select c).FirstOrDefault();

            if (subCate == null)
            {
                TempData["subCate"] = "";
                return Redirect("/Home/Index");
            }
            var cateGory = (from c in _shopContext.Categories
                            where c.CategoryId == subCate.CateogoryId
                            select c).First();

            if (cateGory == null)
            {
                TempData["subCate"] = "";
                return Redirect("/Home/Index");
            }
            if (cateGory != null)
            {
                var e = _shopContext.Entry(cateGory);
                e.Collection(c => c.SubCategories).Load();
                subCategories = cateGory.SubCategories;
            }
            subCategories.Sort((x, y) => x.SubCategoryId.CompareTo(y.SubCategoryId));
            ViewData["id"] = id;
            ViewData["listSubCate"] = subCategories;
            ViewData["subCate"] = subCate;

            if (mode == 1)
            {
                list.Sort((x, y) => (x.ProductPrice - x.Discount).CompareTo(y.ProductPrice - y.Discount));
            }
            if (mode == 2)
            {
                list.Sort((x, y) => (y.ProductPrice - y.Discount).CompareTo(x.ProductPrice - x.Discount));
            }
            return View(list);

        }

        public IActionResult DetailProduct(int id)
        {
            var product = _shopContext.Products.Include(p => p.Feedbacks).Where(p => p.ProductId == id).FirstOrDefault();



            if (product != null)
            {
                ViewData["productId"] = product.ProductId;
                ViewData["image"] = product.ImageMain;
                var e = _shopContext.Entry(product);
                e.Collection(c => c.ImageProducts).Load();
                e.Collection(c => c.ProductDetails).Load();
            }
            else
            {
                return RedirectToAction("Index", "Home", new { mode = "EDetailProduct" });
            }


            int check = 0;

            foreach (var item in _shopContext.productdetails.Where(p => p.productId == id))
            {
                if (item.quantity > 0)
                {
                    check = 1;
                    break;
                }
            }

            if (check == 0)
            {
                product.IsAvailble = false;
                _shopContext.SaveChanges();
            }


            string size = "";
            string color = "";
            var list = new List<string>();
            foreach (var l in product.ProductDetails)
            {
                if (l.quantity > 0)
                {
                    size = l.size;
                    color = l.color;
                    break;
                }
            }

            foreach (var l in product.ProductDetails)
            {
                if (l.color.Equals(color) && l.quantity == 0)
                {
                    list.Add(l.size);
                }
            }
            ViewData["list"] = list;
            ViewData["c"] = color;
            ViewData["s"] = size;

            return View(product);
        }

        public IActionResult Search(string name)
        {
            if (string.IsNullOrEmpty(name))
                return View(null);

            _logger.LogError(_shopContext.Products.Where(c => c.ProductName.Contains(name)).ToList().Count() + "");

            return View("Index", _shopContext.Products.Where(c => c.ProductName.Contains(name)).ToList());
        }


        public IActionResult AddWishList(int id, int choice)
        {
            string? cookieValue = Request.Cookies["wish"];
            if (cookieValue == null)
            {
                var option = new CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(90)
                };
                Response.Cookies.Append("wish", id + ",", option);
            }
            else
            {

                var option = new CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(90)
                };
                Response.Cookies.Append("wish", cookieValue + id + ",", option);
            }


            if (choice == 1)
                return Redirect("/Home/Index");
            string CompleteUrl = Request.Headers["Referer"].ToString();

            return Redirect(CompleteUrl);
         

        }


        public IActionResult RemoveWishList(int id)
        {


            List<int> wishList = new List<int>();
            string? cookieValue = Request.Cookies["wish"];
            if (cookieValue != null)
            {
                foreach (var c in cookieValue.Split(","))
                {
                    if (!string.IsNullOrEmpty(c))
                        wishList.Add(int.Parse(c));
                }
                wishList.RemoveAll(item => item == id);
                if (wishList.Count == 0)
                {
                    Response.Cookies.Delete("wish");
                }
                else
                {
                    string temp = "";
                    foreach (var c in wishList)
                    {
                        temp = temp + c + ",";
                    }

                    var option = new CookieOptions()
                    {
                        Expires = DateTime.Now.AddDays(90)
                    };
                    Response.Cookies.Append("wish", temp, option);
                }
            }


            return Redirect("/Product/WishList");



        }

        public IActionResult WishList()
        {
            List<int> list = new List<int>();


            string? cookieValue = Request.Cookies["wish"];
            if (cookieValue != null)
            {
                foreach (var c in cookieValue.Split(","))
                {
                    if (!string.IsNullOrEmpty(c))
                        list.Add(int.Parse(c));
                }
            }

            return View(_shopContext.Products.Where(c => list.Contains(c.ProductId)).ToList());
        }



    }
}
