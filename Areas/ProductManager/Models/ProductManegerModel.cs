using System.ComponentModel.DataAnnotations;
using WingtipToys.Models;

namespace WingtipToys.Areas.ProductManager
{
    public class ProductManegerModel : Product
    {
        [Required(ErrorMessage = "Phải chọn file upload")]
        [DataType(DataType.Upload)]
        [Display(Name = "select file upload")]
        public IFormFile FileUpload { get; set; }

        [FileExtensions(Extensions = "png,jpg,jpeg,gif")]
        public string UpLoadFileName => FileUpload?.FileName;
    }
}