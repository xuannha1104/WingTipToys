using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WingtipToys.Models;

namespace WingtipToys.Admin.Role
{
    [Authorize(Roles="admin")] 
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> _roleManager, ProductContext _productContext) : base(_roleManager, _productContext)
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

        public IdentityRole role { get; set; }

        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid==null) return NotFound("role not found!");

            role =await roleManager.FindByIdAsync(roleid);
            if(role != null)
            {
                Input = new InputModel()
                {
                    name = role.Name
                };
                return Page();
            }
            return NotFound("role not found!");

        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid==null) return NotFound("role not found!");
            role =await roleManager.FindByIdAsync(roleid);
            if(role == null) return NotFound("role not found!");

            if (!ModelState.IsValid)
            {
                return Page();
            }
            role.Name = Input.name;
            var result = await roleManager.UpdateAsync(role);
            if(result.Succeeded )
            {
                StatusMessage =$"{Input.name} role is updated!";
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