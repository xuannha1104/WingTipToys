using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WingtipToys.Models
{
    public class Category
    {
        [ScaffoldColumn(false)]
        [Key]
        public int CategoryID { get; set; }

        
        [Required(ErrorMessage = "Category Name is required!")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} length from {1} to {2} characters.")]
        [Display(Name = "Name")]
        public string CategoryName { get; set; }

        [Display(Name = "Description")]
        [Column(TypeName="ntext")]
        public string Description { get; set; }

        public  ICollection<Product> Products { get; set; }
    }
}