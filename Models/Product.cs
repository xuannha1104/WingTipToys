using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WingtipToys.Models
{
    public class Product
    {
        [ScaffoldColumn(false)]
        [Key]
        public int ProductID { get; set; }

        [Required, StringLength(100), Display(Name = "Name")]
        public string ProductName { get; set; }

        [Required, StringLength(10000), Display(Name = "Product Description"), DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string ImagePath { get; set; }

        [Display(Name = "Price")]
        [Column(TypeName="Money")]
        public double? UnitPrice { get; set; }

        public int? CategoryID { get; set; }

        public  Category Category { get; set; }
    }
}