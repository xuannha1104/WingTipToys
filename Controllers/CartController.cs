using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly ProductContext _context;

        private readonly CartService _cartService;

        private int _categoryId;

        public CartController(ILogger<CartController> logger,ProductContext context,CartService cartService)
        {
            _logger = logger;
            _context = context;
            _cartService = cartService;

        }

        public IActionResult AddToCart(int productid)
        {
            var product = _context.Products.Where(p => p.ProductID == productid).FirstOrDefault();
            if (product == null)
            {
                return NotFound("the Product is Not Found!");
            }
            
            // Xử lý đưa vào Cart ...
            var cart = _cartService.GetCartItems();
            var cartItem = cart.Find(p => p.product.ProductID == productid);
            if (cartItem != null)
            {
                cartItem.quantity ++;
            }
            else
            {
                cart.Add(new CartItem() {quantity = 1, product = product});
            }
            // _categoryId = FindCategoryIdByProduct(productid);
            // Lưu cart vào Session
            _cartService.SaveCartSession(cart);

            // Chuyển đến trang hiện thị Cart
            return RedirectToAction(nameof(cart));
        }
        // private int FindCategoryIdByProduct(int id)
        // {
        //     var product = _context.Products.Where(p => p.ProductID == id)
        //                 .Include(p => p.Category)
        //                 .FirstOrDefault();
        //     if (product == null) return 0;

        //     return product.Category.CategoryID;
        // }

        [Route ("/cart", Name = "cart")]
        public IActionResult Cart () 
        {
            // ViewBag.categoryID = _categoryId;
            return View (_cartService.GetCartItems());
        }
        [Route ("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart(int productid)
        {
            var cart = _cartService.GetCartItems();
            var cartItem = cart.Find(p => p.product.ProductID == productid);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
            }
            _cartService.SaveCartSession(cart);
            return RedirectToAction(nameof(cart));
        }

        [HttpPost]
        [Route ("/updatecart", Name = "updatecart")]
        public IActionResult UpdateCart([FromForm]int productid,[FromForm] int quantity)
        {
            // return Ok("it's work!");
            var cart = _cartService.GetCartItems();
            var cartItem = cart.Find(p => p.product.ProductID == productid);
            if (cartItem != null)
            {
                cartItem.quantity = quantity;
            }
            _cartService.SaveCartSession(cart);
            return Ok();
        }

        public IActionResult CheckOut()
        {
            var cart = _cartService.GetCartItems();
            // ...
            _cartService.ClearCart();
            return Content("");
        }

    }
}