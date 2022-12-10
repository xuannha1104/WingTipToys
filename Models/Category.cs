using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WingtipToys.Models
{
    public class Category
    {
        [ScaffoldColumn(false)]
        [Key]
        public int CategoryID { get; set; }

        [Required, StringLength(100), Display(Name = "Name")]
        public string CategoryName { get; set; }

        [Display(Name = "Product Description")]
        [Column(TypeName="ntext")]
        public string? Description { get; set; }

        public  ICollection<Product> Products { get; set; }
    }
}