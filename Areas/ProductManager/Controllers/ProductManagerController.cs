using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Areas.ProductManager
{
    [Area("ProductManager")]
    [Authorize(Roles ="admin")]
    [Route("admin/products/[action]/{productid?}")]
    public class ProductManagerController : Controller
    {
        private ProductContext _context;

        [TempData]
        public string statusMessage { get; set; }

        public ProductManagerController(ProductContext context)
        {
            _context = context;
        }
        public class UploadOneFile
        {
        //     [Required(ErrorMessage = "Phải chọn file upload")]
        //     [DataType(DataType.Upload)]
        //     [FileExtensions(Extensions = "png,jpg,jpeg,gif")]
        //     [Display(Name = "select file upload")]
            public IFormFile FileUpload { get; set; }
        }

        public async Task<IActionResult> Index()
        {
            List<Product> allProducts = await (from p in _context.Products 
                                        orderby p.CategoryID,p.ProductID select p)
                                        .Include(p => p.Category)
                                        .ToListAsync();
                                        
            return View(allProducts);
        }

        public async Task<IActionResult> Create()
        {
            var categories =await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "CategoryID", "CategoryName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("ProductName,UnitPrice,FileUpload,Description,CategoryID")] ProductManegerModel product)
        {
            var categories =await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "CategoryID", "CategoryName");

            if (ModelState.IsValid)
            {
                if(product.FileUpload == null) return View();

                var file1 = product.ProductName + Path.GetExtension(product.FileUpload.FileName);
                var file = Path.Combine("wwwroot","Images", "Products", file1);
                var addProduct = new Product(){
                    ProductName = product.ProductName,
                    Description = product.Description,
                    ImagePath = file1,
                    UnitPrice = product.UnitPrice,
                    CategoryID = product.CategoryID
                };
                await _context.AddAsync(addProduct);
                await _context.SaveChangesAsync();

                using (var filestream = new FileStream(file, FileMode.OpenOrCreate))
                {
                    product.FileUpload.CopyTo(filestream);
                }
                ViewData["StatusMessage"] = "New product has created!";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Details(int? productid)
        {
            if (productid == null) return NotFound("Product ID not found!");

            var product =await _context.Products.Where(p => p.ProductID == productid).FirstOrDefaultAsync();
            if (product == null) return NotFound("Product is not found!");

            return View(product);
        }

        public async Task<IActionResult> Edit(int? productid)
        {
            if (productid == null) return NotFound("Product ID not found!");

            var product =await _context.Products.Where(p => p.ProductID == productid)
                                                .Include(p => p.Category)
                                                .FirstOrDefaultAsync();
            if (product == null) return NotFound("Product is not found!");

            var categories =await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories,"CategoryID", "CategoryName");
            return View(product);
        }
        
        [HttpPost]
        public async Task<IActionResult>Edit(int? productid,[Bind("ProductName,UnitPrice,Description,CategoryID")] Product product)
        {
            IFormFile uploadPhoto = null;
            string oldeFileName,newFileName = null;
            string uploadFolder = Path.Combine("Uploads", "Products");
            string SaveFolder =Path.Combine("wwwroot","Images", "Products");
            string sourceFile,destinationFile ="";

            if(productid == null ) return NotFound("Product ID is not found!");
            var editProduct = await _context.Products.Where(p => p.ProductID == productid).FirstOrDefaultAsync();
            if (editProduct==null) return NotFound($"Product is not found with ID: {productid}");
            
            string oldProductName  = editProduct.ProductName;
            editProduct.CategoryID = product.CategoryID;
            editProduct.ProductName = product.ProductName;
            editProduct.UnitPrice = product.UnitPrice;
            editProduct.Description = product.Description;

            IFormFileCollection newPhotos = HttpContext.Request.Form.Files;
            if(newPhotos.Count!=0)
            {
                uploadPhoto = newPhotos[0];
                newFileName = product.ProductName + Path.GetExtension(uploadPhoto.FileName);
            }
            else
            {
                newFileName = product.ProductName + Path.GetExtension(editProduct.ImagePath);
            }
            editProduct.ImagePath = newFileName;
            _context.Products.Update(editProduct);
            await _context.SaveChangesAsync();

            if (oldProductName != product.ProductName)
            {
                oldeFileName = oldProductName + Path.GetExtension(editProduct.ImagePath);
                newFileName = product.ProductName + Path.GetExtension(editProduct.ImagePath);
                sourceFile =Path.Combine(SaveFolder, oldeFileName);
                destinationFile = Path.Combine(SaveFolder,newFileName); 
                if (System.IO.File.Exists(sourceFile))
                {
                    System.IO.File.Move(sourceFile,destinationFile);
                }
            }
            
            if (uploadPhoto != null)
            {   string uploadFile =productid.ToString() + Path.GetExtension(uploadPhoto.FileName) ;
                sourceFile =Path.Combine(uploadFolder, uploadFile);
                destinationFile = Path.Combine(SaveFolder,newFileName); 
                
                if (System.IO.File.Exists(sourceFile))
                {

                    System.IO.File.Move(sourceFile,destinationFile,true);
                }
            }
            ViewData["StatusMessage"] = "product has been updated!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ListPhotos(int productid)
        {
            var product = _context.Products.Where(e => e.ProductID == productid)
                .FirstOrDefault();

            if (product == null)
            {
                return Json(
                    new {
                        success = 0,
                        message = "Product not found",
                    }
                );
            }

            var imagePath ="/Images/Products/" + product.ImagePath;

            return Json(
                new { 
                    success = 1,
                    image = imagePath
                }
            );

            
        }

        [HttpPost, ActionName("UpLoadNewPhoto")]
        public async Task<IActionResult> UploadPhotoAsync(int productid, [Bind("FileUpload")]UploadOneFile f)
        {
            var product =await _context.Products.Where(p => p.ProductID == productid).FirstOrDefaultAsync();
            if(product==null) 
            {
                return Json(
                    new {
                        success = 0,
                        message = "Product not found",
                    }
                );
            }

            if(f != null)
             {
                var file1 = productid.ToString()+ Path.GetExtension(f.FileUpload.FileName);

                var file = Path.Combine("Uploads", "Products", file1);
                            
                using (var filestream = new FileStream(file, FileMode.OpenOrCreate))
                {
                    f.FileUpload.CopyTo(filestream);
                }
                return Json(
                    new {
                        success = 1,
                        message = "/contents/Products/" + file1,
                    }
                );
             }
            return Json(
                    new {
                        success = 0,
                        message = "can not update photo",
                    }
            );

        }

        public IActionResult Delete(int? productid)
        {
            if (productid == null) return NotFound("Product ID not found!");

            var delProduct = _context.Products.Where(p => p.ProductID == productid).FirstOrDefault();
            if (delProduct == null) return NotFound($"Product is not found with ID: {productid}");

            return View(delProduct);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? productid)
        {
            if (productid == null) return NotFound("Product ID not found!");

            var delProduct = await _context.Products.Where(p => p.ProductID == productid).FirstOrDefaultAsync();
            if (delProduct == null) return NotFound($"Product is not found with ID: {productid}");

            var imagePath =Path.Combine("wwwroot","Images","Products",delProduct.ImagePath);
            _context.Products.Remove(delProduct);
            await _context.SaveChangesAsync();

            if (System.IO.File.Exists(imagePath))
            {
                try
                {
                    System.IO.File.Delete(imagePath);
                }
                catch(Exception)
                {
                    throw;
                }
            }
            
            ViewData["StatusMessage"] = "New product has removed!";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public JsonResult IsAlreadyExist(string ProductName)
        {
            // return Json(IsProductAvailable(ProductName));
            return Json(false);
        }
        private bool IsProductAvailable(string name)
        {
            var product = _context.Products.Where(p => p.ProductName == name).FirstOrDefault();
            if (product!=null) 
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

}