using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace WingtipToys.Models
{
    public class Product
    {
        [ScaffoldColumn(false)]
        [Key]
        public int ProductID { get; set; }

        [Required, Display(Name = "Name")]
        [StringLength(100,MinimumLength = 3 ,ErrorMessage ="{0} length must from {2} to {2} characters")]
        [Remote( action: "IsAlreadyExist",controller:"ProductManager",areaName:"ProducManager",HttpMethod ="POST",ErrorMessage ="Product name is already existed!")]
        public string ProductName { get; set; }

        [Required, Display(Name = "Product Description"), DataType(DataType.MultilineText)]
        [StringLength(10000,MinimumLength = 6 ,ErrorMessage ="{0} length must from {2} to {2} characters")]
        public string Description { get; set; }
        
        public string ImagePath { get; set; }

        [Display(Name = "Price")]
        [Column(TypeName="Money")]
        public double? UnitPrice { get; set; }

        public int? CategoryID { get; set; }

        public  Category Category { get; set; }
    }
}