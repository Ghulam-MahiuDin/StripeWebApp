using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using StripeWebApp.Data;
using StripeWebApp.Models;

namespace StripeWebApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PaymentController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Items.ToListAsync());
        }
        [HttpPost]
        public ActionResult Create([Bind("Id,Name,ImageUrl,PriceID")] Item item)
        {
            var domain = "https://localhost:7113";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                      Price =$"{item.PriceID}",
                      //Price = "price_1PLfksHaaqG3CBCGar0G1PdA",
                    Quantity = 1,
                  },
                },
                Mode = "payment",
                SuccessUrl = domain + "/Payment/Success",
                CancelUrl = domain + "/Payment/Cancel",
            };
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        public IActionResult Success()
        {
            return View();
        }
        public IActionResult Cancel()
        {
            return View();
        }
    }
}
