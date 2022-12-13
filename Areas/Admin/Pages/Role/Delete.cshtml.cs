using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WingtipToys.Models;

namespace WingtipToys.Admin.Role
{
    [Authorize(Roles="admin")] 
    public class DeleteModel : RolePageModel
    {
        public DeleteModel(RoleManager<IdentityRole> _roleManager, ProductContext _productContext) : base(_roleManager, _productContext)
        {
        }

        public IdentityRole role { get; set; }

        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid==null) return NotFound("role not found!");

            role =await roleManager.FindByIdAsync(roleid);
            if(role == null)
            {
                return NotFound("role not found!");
            }
            
            return Page();
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
            var result = await roleManager.DeleteAsync(role);
            if(result.Succeeded )
            {
                StatusMessage =$"{role.Name} is deleted!";
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