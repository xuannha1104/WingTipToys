using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Areas.ProductManager
{   
    [Area("Product")]
    [Authorize(Roles ="admin")]
    [Route("admin/category/[action]/{id?}")]
    public class CategoryController : Controller
    {
        private ProductContext _context;
        

        public CategoryController(ProductContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories =await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryName,Description")] Category category)
        {
            if(ModelState.IsValid)
            {
                var checkCategory = await _context.Categories.Where(c => c.CategoryName == category.CategoryName)
                                    .FirstOrDefaultAsync();
                if (checkCategory != null)
                {
                    ViewBag.errMessage = $"Error: category {category.CategoryName} has already exist";
                    return View();
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["statusMessage"] =$"success:{category.CategoryName} category has created.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound("CategoryID is not found!");

            var category = await _context.Categories.Where(c => c.CategoryID == id).FirstOrDefaultAsync();
            if (category == null) return NotFound("Category is not found!");
            return View(category);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound("CategoryID is not found!");
            var category = await _context.Categories.Where(c => c.CategoryID == id).FirstOrDefaultAsync();
            if (category == null) return NotFound("Category is not found!");
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id,[Bind("CategoryName,Description")] Category category)
        {
            if(ModelState.IsValid)
            {
                if (id == null) return NotFound("CategoryID is not found!");
                var effectedCategory = await _context.Categories.Where(c => c.CategoryID == id).FirstOrDefaultAsync();
                if (effectedCategory == null) return NotFound("Category is not found!");

                var checkCategory = await _context.Categories
                                    .Where(c => c.CategoryName == category.CategoryName && c.CategoryID != id)
                                    .FirstOrDefaultAsync();
                if (checkCategory != null)
                {
                    ViewBag.ErrMessage =$"Error: category {category.CategoryName} has already exist";
                    return View();
                }
                _context.Update(category);
                await _context.SaveChangesAsync();
                TempData["statusMessage"]  = $"success:{category.CategoryName} category has updated.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound("CategoryID is not found!");
            var delCategory = await _context.Categories.Where(c => c.CategoryID == id).FirstOrDefaultAsync();
            if (delCategory == null) return NotFound("Category is not found!");
            return View(delCategory);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null) return NotFound("CategoryID is not found!");
            var delCategory = await _context.Categories.Where(c => c.CategoryID == id).FirstOrDefaultAsync();
            if (delCategory == null) return NotFound("Category is not found!");
            
            string delCategoryName  = delCategory.CategoryName;
            _context.Remove(delCategory);
            await _context.SaveChangesAsync();

            TempData["statusMessage"] = $"success:{delCategoryName} category has been removed.";
            return RedirectToAction(nameof(Index));
        }
    }
    
}