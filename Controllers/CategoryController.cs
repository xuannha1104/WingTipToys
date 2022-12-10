using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ProductContext _context;

        public CategoryController(ILogger<CategoryController> logger,ProductContext context)
        {
            _logger = logger;
            _context = context;
        }

        //  [Route("/ProductList/{categoryid}")]
        public IActionResult ProductList(int categoryid)
        {
            // ViewBag.categoryName = null;
            // ViewBag.productList = null;
            _logger.LogInformation(categoryid.ToString());
            var category = (from c in _context.Categories where c.CategoryID == categoryid select c)
                    .Include(c => c.Products)
                    .FirstOrDefault();

            if (category == null)
            {
                return NotFound("Not Found!");
            }
            else
            {
                 var productList = category.Products.ToList();
                // if (productList == null)
                // {
                //     return NotFound("Product List not found!");
                // }
                // else
                // {
                //     return Ok("Product list is found!");
                // }
                // ViewBag.category = category;
                // ViewBag.productList = productList;
                ViewBag.categoryName = category.CategoryName;
                ViewBag.productList = productList;
                return View();
            }
            // var productList = category.Products;
             
            // return View(productList);
        }
    }
}