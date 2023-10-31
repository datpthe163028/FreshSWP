using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Utilities;
using PayPal.Api;
using Project.Data;
using Project.Models;
using System.Globalization;

namespace Project.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        private readonly ShopContext _shopContext;
        private readonly SignInManager<IdentityUser> _SignInManager;
        public OrderController(ILogger<OrderController> logger, ShopContext ct, SignInManager<IdentityUser> SignInManager)
        {
            _shopContext = ct;
            _logger = logger;
            _SignInManager = SignInManager;
        }

        public IActionResult Index(IFormCollection f)
        {
            var products = _shopContext.Products.ToList();
            double? total = 0;


            foreach (var cartItem in getListItem())
            {
                foreach (var product in products)
                {
                    if (product.ProductId == cartItem.ProductId)
                    {
                        total = total + cartItem.Quantity * (product.ProductPrice - product.Discount);
                    }
                }
            }

            ViewData["products"] = products;
            ViewData["total"] = total;
            return View(getListItem());
        }


        [HttpPost]
        public IActionResult ProcessOrder(string email, string total, string payment, string address, string phone)
        {


            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Không được để rỗng";
                return RedirectToAction("Index", "Order");
            }

            if (string.IsNullOrEmpty(phone) || phone.Length < 9)
            {
                TempData["Errorr"] = "Sai định dạng";
                return RedirectToAction("Index", "Order");
            }

            if (int.TryParse(phone, out _) == false)
            {
                TempData["Errorr"] = "Sai định dạng";
                return RedirectToAction("Index", "Order");
            }

            if (payment == "2")
            {
                TempData["publicemail"] = email;
                TempData["publictotal"] = total;
                decimal totalAmount;
                if (decimal.TryParse(total, out totalAmount))
                {
                    decimal totalAmountVND = totalAmount * 1000;
                    decimal exchangeRate = 23745;
                    decimal totalAmountUSD = totalAmountVND / exchangeRate;


                    return RedirectToAction("PaymentWithPayPal", "Order", new { total = totalAmountUSD });

                }
                else
                {
                    return RedirectToAction("Index", "Order");

                }
            }
            else if (payment == "1")
            {
                if (string.IsNullOrEmpty(address))
                {
                    TempData["ErrorA"] = "Thêm địa chỉ nhận hàng";
                    return RedirectToAction("Index", "Order");
                }
                var newBill = CreateBill(email, total, "1", address);
                if (ViewData.ContainsKey("bill"))
                {
                    ViewData.Remove("bill");
                }
                if (newBill != null)
                {
                    ViewData["bill"] = newBill;
                }

            }
            else
            {
                return RedirectToAction("Index", "Order");
            }



            return View();
        }
        public IActionResult ProcessOrder()
        {

            return View();
        }
        private Bill CreateBill(string email, string totalAmount, string paymentCode, string address)
        {
            var RoldId = _shopContext.Roles.Where(c => c.NormalizedName == "SELLER").FirstOrDefault();
            List<string> lSellerId = _shopContext.UserRoles.Where(c => c.RoleId == RoldId.Id).Select(c => c.UserId).ToList();
            Dictionary<string, int> myDictionary = new Dictionary<string, int>();

            foreach (var sellerId in lSellerId)
            {
                myDictionary.Add(sellerId, 0);
            }

            foreach (var b in _shopContext.Bills.Where(c => c.BillStatus == "0").Select(c => c.sellerId).ToList())
            {
			   var pair = myDictionary.FirstOrDefault(x => x.Key == b);

				if (!pair.Equals(default(KeyValuePair<string, int>)))
				{
					myDictionary[pair.Key] = myDictionary[pair.Key] + 1;
				}
			}

            var minPair = myDictionary.MinBy(pair => pair.Value);

            Bill newBill;
            if (paymentCode == "2")
            {
                if (_SignInManager.IsSignedIn(User))
                {
                    newBill = new Models.Bill()
                    {
                        sellerId = minPair.Key,
                        Email = email,
                        UserId = _SignInManager.UserManager.GetUserId(User),
                        TransportId = 1,
                        BillStatus = "0",
                        PaymentCode = int.Parse(paymentCode),
                        PurchaseDate = DateTime.Now,
                        PaymentMethod = "paypal",
                        ShippingAddress = address,
                        ShippingFee = 0,
                        TotalPrice = double.Parse(totalAmount)
                    };
                }
                else
                {
                    newBill = new Models.Bill()
                    {
                        sellerId = minPair.Key,
                        Email = email,
                        TransportId = 1,
                        BillStatus = "0",
                        PaymentCode = int.Parse(paymentCode),
                        PurchaseDate = DateTime.Now,
                        PaymentMethod = "paypal",
                        ShippingAddress = address,
                        ShippingFee = 0,
                        TotalPrice = double.Parse(totalAmount)
                    };
                }
            }
            else
            {
                if (_SignInManager.IsSignedIn(User))
                {
                    newBill = new Models.Bill()
                    {
                        sellerId = minPair.Key,
                        Email = email,
                        UserId = _SignInManager.UserManager.GetUserId(User),
                        TransportId = 1,
                        BillStatus = "0",
                        PaymentCode = int.Parse(paymentCode),
                        PurchaseDate = DateTime.Now,
                        PaymentMethod = "cod",
                        ShippingAddress = address,
                        ShippingFee = 0,
                        TotalPrice = double.Parse(totalAmount)
                    };
                }
                else
                {
                    newBill = new Models.Bill()
                    {
                        sellerId = minPair.Key,
                        Email = email,
                        TransportId = 1,
                        BillStatus = "0",
                        PaymentCode = int.Parse(paymentCode),
                        PurchaseDate = DateTime.Now,
                        PaymentMethod = "cod",
                        ShippingAddress = address,
                        ShippingFee = 0,
                        TotalPrice = double.Parse(totalAmount)
                    };
                }
            }

            _shopContext.Bills.Add(newBill);
            _shopContext.SaveChanges();

            foreach (var cartItem in GetCartItems())
            {
                _shopContext.BillDetails.Add(new BillDetail()
                {
                    BillId = newBill.BillId,
                    ProductId = cartItem.ProductId,
                    quantity = cartItem.Quantity,
                    color = cartItem.color,
                    size = cartItem.size
                });
                if (cartItem.CartItemId >= 0)
                {
                    _shopContext.CartItems.Remove(cartItem);
                }
            }

            _shopContext.SaveChanges();
            Response.Cookies.Delete("cart");
            return newBill;
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
                cartItems = _shopContext.CartItems.Include(i => i.product).Where(i => i.CartId == cartId).ToList();
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


        public List<CartItem> GetCartItems()
        {
            List<CartItem> cartItems = new List<CartItem>();

            if (_SignInManager.IsSignedIn(User))
            {
                string userId = _SignInManager.UserManager.GetUserId(User);
                int cartId = _shopContext.Carts
                    .Where(c => c.UserId == userId)
                    .Select(c => c.CartId)
                    .FirstOrDefault();
                cartItems = _shopContext.CartItems
                    .Include(ci => ci.product)
                    .Where(ci => ci.CartId == cartId)
                    .ToList();
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
        private Payment CreatePayment(decimal totalAmount)
        {
            var apiContext = new APIContext(new OAuthTokenCredential("Abx8Hd9L7RUBlJdah8Fi5neref-JUamajEJWhcan_cR16sMYrEUTr_N7o8QUTWY3a-4S7fB914rHd3c2",
                "EKaabtbIsmSo1Zmd-dY2fXOekaxfUJBRvjjpUJIDiY7b1d1tMZZ5CbiqTT1mQ4K-hIqp69VkAx8fF9Hi").GetAccessToken());

            var payment = new Payment()
            {
                intent = "sale",
                payer = new Payer() { payment_method = "paypal" },
                transactions = new List<Transaction>()
        {
            new Transaction()
            {
                amount = new Amount()
                {
                    currency = "USD",
                    total = totalAmount.ToString("F2")
                },
                description = "Payment for Order"
            }
        },
                redirect_urls = new RedirectUrls()
                {
                    return_url = Url.Action("ExecutePayment", "Order", null, Request.Scheme),
                    cancel_url = Url.Action("CancelPayment", "Order", null, Request.Scheme)
                }
            };

            var createdPayment = payment.Create(apiContext);
            return createdPayment;
        }

        public IActionResult PaymentWithPayPal(decimal total)
        {
            var payment = CreatePayment(total);
            var approvalUrl = payment.links.FirstOrDefault(x => x.rel.ToLower() == "approval_url")?.href;

            if (!string.IsNullOrEmpty(approvalUrl))
            {
                return Redirect(approvalUrl);
            }

            // Xử lý khi không có URL phê duyệt.
            return View("FailureView");
        }

        public async Task<IActionResult> ExecutePayment(string paymentId, string token, string PayerID)
        {
            var apiContext = new APIContext(new OAuthTokenCredential("Abx8Hd9L7RUBlJdah8Fi5neref-JUamajEJWhcan_cR16sMYrEUTr_N7o8QUTWY3a-4S7fB914rHd3c2",
                "EKaabtbIsmSo1Zmd-dY2fXOekaxfUJBRvjjpUJIDiY7b1d1tMZZ5CbiqTT1mQ4K-hIqp69VkAx8fF9Hi").GetAccessToken());

            var paymentExecution = new PaymentExecution() { payer_id = PayerID };
            var executedPayment = new Payment() { id = paymentId }.Execute(apiContext, paymentExecution);

     
            var shippingAddress = executedPayment.transactions[0].item_list.shipping_address;
            string addressString = $"{shippingAddress.line1}, {shippingAddress.line2}, {shippingAddress.city}, {shippingAddress.state}, {shippingAddress.postal_code}, {shippingAddress.country_code}";

            string publicemail = TempData["publicemail"] as string;
            string publictotal = TempData["publictotal"] as string;
            var newBill = CreateBill(publicemail, publictotal, "2", addressString);

            if (ViewData.ContainsKey("bill"))
            {
                ViewData.Remove("bill");
            }
            if (newBill != null)
            {
                ViewData["bill"] = newBill;
            }

            return View("ProcessOrder");
        }
    }


}
