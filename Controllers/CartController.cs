using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Project.Data;
using Project.Models;
using System.Drawing;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;

namespace Project.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShopContext _shopContext;
        private readonly SignInManager<IdentityUser> _SignInManager;
        public CartController(ILogger<HomeController> logger, ShopContext ct, SignInManager<IdentityUser> SignInManager)
        {
            _shopContext = ct;
            _logger = logger;
            _SignInManager = SignInManager;
        }

        public IActionResult Index(string choice, int id)
        {
            List<CartItem> cartItems = new List<CartItem>();



            if (string.IsNullOrEmpty(choice) || id == 0)
            {
                cartItems = getListItem();

            }
            else
            {
                if (choice.Equals("remove"))
                {
                    if (id >= 0)
                    {
                        var item = _shopContext.CartItems.Find(id);
                        if (item != null)
                        {
                            _shopContext.CartItems.Remove(item);
                            _shopContext.SaveChanges();
                        }
                        cartItems = getListItem();
                    }
                    else
                    {
                        string? cookieValue = Request.Cookies["cart"];
                        if (cookieValue != null)
                        {
                            cartItems = getListItem();
                            List<CartItem> cartListFromCookie = JsonConvert.DeserializeObject<List<CartItem>>(cookieValue);
                            var cartItem = cartListFromCookie.Find(p => p.CartItemId == id);
                            if (cartItem != null)
                            {
                                cartListFromCookie.Remove(cartItem);
                                cartItems.Remove(cartItems.Find(p => p.CartItemId == id));
                            }
                            if (cartListFromCookie.Count == 0 || cartListFromCookie == null)
                            {
                                Response.Cookies.Delete("cart");
                            }
                            else
                            {
                                string json = JsonConvert.SerializeObject(cartListFromCookie);
                                var option = new CookieOptions()
                                {
                                    Expires = DateTime.Now.AddDays(90)
                                };
                                Response.Cookies.Append("cart", json, option);
                            }
                        }
                    }
                }

                if (choice.Equals("up"))
                {
                    if (id >= 0)
                    {
                        var item = _shopContext.CartItems.Find(id);
                        if (item != null)
                        {
                            item.Quantity = item.Quantity + 1;
                            _shopContext.SaveChanges();
                        }
                        cartItems = getListItem();
                    }
                    else
                    {
                        string? cookieValue = Request.Cookies["cart"];
                        if (cookieValue != null)
                        {
                            cartItems = getListItem();
                            List<CartItem> cartListFromCookie = JsonConvert.DeserializeObject<List<CartItem>>(cookieValue);
                            var cartItem = cartListFromCookie.Find(p => p.CartItemId == id);
                            var cartItem2 = cartItems.Find(p => p.CartItemId == id);
                            if (cartItem != null)
                            {
                                //cartListFromCookie.Remove(cartItem);
                                //cartItems.Remove(cartItems.Find(p => p.CartItemId == id));
                                cartItem.Quantity = cartItem.Quantity + 1;
                                if (cartItem2 != null)
                                    cartItem2.Quantity = cartItem2.Quantity + 1;
                            }


                            string json = JsonConvert.SerializeObject(cartListFromCookie);
                            var option = new CookieOptions()
                            {
                                Expires = DateTime.Now.AddDays(90)
                            };
                            Response.Cookies.Append("cart", json, option);
                        }
                    }
                }


                if (choice.Equals("down"))
                {
                    if (id >= 0)
                    {
                        var item = _shopContext.CartItems.Find(id);
                        if (item != null)
                        {
                            item.Quantity = item.Quantity - 1;
                            _shopContext.SaveChanges();
                        }
                        cartItems = getListItem();
                    }
                    else
                    {
                        string? cookieValue = Request.Cookies["cart"];
                        if (cookieValue != null)
                        {
                            cartItems = getListItem();
                            List<CartItem> cartListFromCookie = JsonConvert.DeserializeObject<List<CartItem>>(cookieValue);
                            var cartItem = cartListFromCookie.Find(p => p.CartItemId == id);
                            var cartItem2 = cartItems.Find(p => p.CartItemId == id);
                            if (cartItem != null)
                            {
                                if (cartItem.Quantity != 1)
                                {
                                    cartItem.Quantity = cartItem.Quantity - 1;
                                    if (cartItem2 != null)
                                        cartItem2.Quantity = cartItem2.Quantity - 1;
                                }

                            }


                            string json = JsonConvert.SerializeObject(cartListFromCookie);
                            var option = new CookieOptions()
                            {
                                Expires = DateTime.Now.AddDays(90)
                            };
                            Response.Cookies.Append("cart", json, option);
                        }
                    }
                }
            }

            return View(getListItem());
        }

        [HttpPost]
        public IActionResult Index(IFormCollection f)
        {
           

            var color = f["color"];
            var size = f["size"];
            var qua = int.Parse(f["Quantity"]);
            int pId = int.Parse(f["productId"]);

            if (string.IsNullOrEmpty(color) || string.IsNullOrEmpty(size))
                return Redirect("/Home/Index");

            List<CartItem> cartItems = new List<CartItem>();
            if (_SignInManager.IsSignedIn(User))
            {
                string userId = _SignInManager.UserManager.GetUserId(User);
                int cartId = (from c in _shopContext.Carts
                              where c.UserId == userId
                              select c.CartId).FirstOrDefault();

                _shopContext.CartItems.Add(new Models.CartItem() { CartId = cartId, ProductId = pId, color = color, Quantity = qua, size = size });
                _shopContext.SaveChanges();
            }
            else
            {
                string? cookieValue = Request.Cookies["cart"];
                if (!string.IsNullOrEmpty(cookieValue))
                {
                    List<CartItem> cartListFromCookie = JsonConvert.DeserializeObject<List<CartItem>>(cookieValue);
                    if (cartListFromCookie.Count != 0)
                    {
                        cartListFromCookie = cartListFromCookie.OrderBy(p => p.CartItemId).ToList();
                        var cartItemId = cartListFromCookie[cartListFromCookie.Count - 1].CartItemId;
                        if (cartItemId == 0)
                        {
                            Response.Cookies.Delete("cart");
                            return RedirectToAction("Index", "Home");
                        }

                        cartListFromCookie.Add(new Models.CartItem() { CartItemId = cartItemId + 1, ProductId = pId, color = color, Quantity = qua, size = size });
                        cartItems.Add((new Models.CartItem() { CartItemId = cartItemId + 1, ProductId = pId, color = color, Quantity = qua, size = size }));

                        string json = JsonConvert.SerializeObject(cartListFromCookie);
                        var option = new CookieOptions()
                        {
                            Expires = DateTime.Now.AddDays(90)
                        };
                        Response.Cookies.Append("cart", json, option);
                    }
                }
                else
                {
                    CartItem cartItem = new Models.CartItem() { CartItemId = -999999999, ProductId = pId, color = color, Quantity = qua, size = size };
                    List<CartItem> saveCookie = new List<CartItem>() { cartItem };
                    string json = JsonConvert.SerializeObject(saveCookie);

                    var option = new CookieOptions()
                    {
                        Expires = DateTime.Now.AddDays(90)
                    };

                    Response.Cookies.Append("cart", json, option);
                    cartItems.Add(cartItem);
                }
            }
            cartItems.AddRange(getListItem());
            string referrerUrl = Request.Headers["Referer"].ToString();
            return Redirect(referrerUrl);
        }

        public List<CartItem> getListItem()
        {
            List<CartItem> cartItems = new List<CartItem>();

            if (_SignInManager.IsSignedIn(User))
            {
                string userId = _SignInManager.UserManager.GetUserId(User);
                int cartId = (from c in _shopContext.Carts
                              where c.UserId == userId
                              select c.CartId).FirstOrDefault();
                var temp = _shopContext.CartItems.Include(i => i.product).Where(i => i.CartId == cartId);


                if (temp != null)
                {
                    cartItems = temp.ToList();
                }

            }

            string? cookieValue = Request.Cookies["cart"];
            if (cookieValue != null)
            {
                List<CartItem> cartListFromCookie = JsonConvert.DeserializeObject<List<CartItem>>(cookieValue);
                cartItems.AddRange(cartListFromCookie);
            }
            cartItems = cartItems.OrderByDescending(p => p.CartItemId).ToList();

            return cartItems;
        }
        public IActionResult ProcessBill(string status)
        {
            var user = HttpContext.User;

            if (User.Identity.IsAuthenticated)
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy ID của người dùng từ Claims

                var bills = _shopContext.Bills
                    .Include(b => b.User)
                    .Include(b => b.BillDetails)
                    .ThenInclude(bd => bd.Product)
                    .Where(b => b.UserId == userId);
                ViewData["check"] = "-1";

                if (!string.IsNullOrEmpty(status))
                {
                    if (status == "0")
                    {
                        bills = bills.Where(b => b.BillStatus == "0");
                        ViewData["check"] = "0";
                    }
                    else if (status == "1")
                    {
                        bills = bills.Where(b => b.BillStatus == "1");
                        ViewData["check"] = "1";
                    }
                    else if (status == "2")
                    {
                        bills = bills.Where(b => b.BillStatus == "2");
                        ViewData["check"] = "2";
                    }
                    else if (status == "3")
                    {
                        bills = bills.Where(b => b.BillStatus == "3");
                        ViewData["check"] = "3";
                    }

                }

                var billList = bills.OrderByDescending(p => p.BillId).ToList();

                if (billList.Count == 0)
                {
                    return View(null);
                }

                return View(billList);
            }
            else
            {
                return Redirect("/Identity/Account/Login");
            }
        }
        public IActionResult processbillfb(string status)
        {
            var user = HttpContext.User;

            if (User.Identity.IsAuthenticated)
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy ID của người dùng từ Claims

                var bills = _shopContext.Bills
                    .Include(b => b.User)
                    .Include(b => b.BillDetails)
                    .ThenInclude(bd => bd.Product)
                    .Where(b => b.UserId == userId);

                if (!string.IsNullOrEmpty(status))
                {
                    if (status == "3")
                    {
                        bills = bills.Where(b => b.BillStatus == "3");
                    }
                    else
                    {
                        return NotFound();
                    }

                }

                var billList = bills.OrderByDescending(p => p.BillId).ToList();

                if (billList.Count == 0)
                {
                    return View(null);
                }

                return View(billList);
            }
            else
            {
                return Redirect("/Identity/Account/Login");
            }
        }



    }
}