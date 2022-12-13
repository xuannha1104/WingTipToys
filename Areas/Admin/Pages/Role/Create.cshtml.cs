using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WingtipToys.Models;

namespace WingtipToys.Admin.Role
{
    [Authorize(Roles="admin")] 
    public class CreateModel : RolePageModel
    {
        public CreateModel(RoleManager<IdentityRole> _roleManager, ProductContext _productContext) : base(_roleManager, _productContext)
        {
        }

        public class InputModel
        {
            [Display(Name ="Role Name")]
            [Required(ErrorMessage ="{0} is input required field!")]
            [StringLength(256,MinimumLength = 3,ErrorMessage ="{0}'s length from {2} to {1}")]
            public string name { get; set; }

        }
        [BindProperty]
        public InputModel Input { get; set;}

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var newRole = new IdentityRole(Input.name);
            var result = await roleManager.CreateAsync(newRole);
            if(result.Succeeded )
            {
                StatusMessage =$"{Input.name} role is created!";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(string.Empty,error.Description);
                });
            }

            return Page();
        }
    }
}