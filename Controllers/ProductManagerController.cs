using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Controllers
{
    public class ProductManagerController : Controller
    {
        private readonly ILogger<ProductManagerController> _logger;
        private readonly ProductContext _context;
        
        [TempData]
        public string statusMessage { get; set; }

        public ProductManagerController(ILogger<ProductManagerController> logger,ProductContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index([FromQuery(Name = "p")]int currentPage, int pageSize)
        {
            var products = _context.Products
                            .Include(p => p.Category)
                            .OrderByDescending(p => p.ProductID)
                            .GroupBy(p => p.CategoryID);

            int productCount = await products.CountAsync();
            if(pageSize<=0) pageSize = 10;

            int pageCount = (int)Math.Ceiling((double)productCount/pageSize);
            if (currentPage>pageCount) currentPage = pageCount;
            if (currentPage<1) currentPage = 1;

            var pagingModel = new PagingModel()
            {
                countpages = pageCount,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index",new{
                    p = pageNumber,
                    pagesize = pageSize
                })
            };
            ViewBag.pagingModel = pagingModel;
            ViewBag.totalProducts = productCount;
            ViewBag.productIndex = (currentPage - 1) * pageSize;

            var productInPage = await products.Skip((currentPage-1)*pageSize)
                                .Take(pageSize)
                                .ToListAsync();

                                



            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }
        public IActionResult Update()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}