using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ShopContext _shopContext;
        private readonly UserManager<IdentityUser> _userManager;

        public FeedbackController(ShopContext shopContext, UserManager<IdentityUser> userManager)
        {
            _shopContext = shopContext;
            _userManager = userManager;

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int billId, int productId, string feedbackTitle, string feedbackDetail)
        {
            var bill = _shopContext.Bills.Include(b => b.BillDetails).FirstOrDefault(b => b.BillId == billId);
            if (bill == null)
            {
                return NotFound();
            }

            var billDetail = bill.BillDetails.FirstOrDefault(bd => bd.ProductId == productId);
            if (billDetail != null && !billDetail.IsFeedbackSubmitted)
            {
                
                if(bill.UserId == null)
                {
                    return NotFound();
                }
                // Create a new feedback object
                var feedback = new Feedback
                {
                    UserId = bill.UserId,
                    FeedbackDate = DateTime.Now,
                    FeedbackTitle = feedbackTitle,
                    FeedbackDetail = feedbackDetail,
                    ProductId = productId,
                    FeedbackStatus = "0"
                };

                // Save the feedback to the database
                _shopContext.Feedbacks.Add(feedback);
                _shopContext.SaveChanges();

                // Mark the bill detail as feedback submitted
                billDetail.IsFeedbackSubmitted = true;
                _shopContext.SaveChanges();
            }

            return RedirectToAction("ProcessBill","Cart");
        }

    }
}
