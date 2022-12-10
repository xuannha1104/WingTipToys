using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ProductContext _context;

        // private readonly CartService _cartService;
        public ProductController(ILogger<ProductController> logger,ProductContext context)
        {
            _logger = logger;
            _context = context;
        }
        // [Route("/products/{CategoryiId}")]
        public IActionResult Detail(int productid)
        {
            var product = (from p in _context.Products where p.ProductID == productid select p)
                        .Include(p=> p.Category)
                        .FirstOrDefault();
            if (product == null)
            {
                return NotFound("product not found");
            }
            else
            {
                if (product.Category == null)
                {
                    return NotFound("khong tim thay category tuong ung!");
                }
                else
                {
                    ViewBag.product = product;
                    ViewBag.categoryID = product.Category.CategoryID;
                    return View();
                }
                
            }
            
        }
    }
}