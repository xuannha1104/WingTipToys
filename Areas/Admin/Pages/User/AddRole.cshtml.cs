using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Admin.User
{
    public class AddRoleModel: PageModel
    {
        private RoleManager<IdentityRole> _roleManager;

        private UserManager<AppUser> _userManager;

        public AppUser user { get; set; }

        [BindProperty]
        [Display(Name ="User's Roles")]
        public string[] RoleNames { get; set; }

        public SelectList AllRoles { get; set; }
        
        [TempData]
        public string StatusMessage { get; set; }
        public AddRoleModel(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager) 
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return NotFound("User not found!");
            }
            user =await _userManager.FindByIdAsync(id);
            if(user == null) return NotFound("User not found!");

            RoleNames = (await _userManager.GetRolesAsync(user)).ToArray<string>();
            List<String> roleList  = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            AllRoles = new SelectList(roleList);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if(string.IsNullOrEmpty(id)) return NotFound("User not found!");

            user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound($"User not found with id: {id}");

            List<String> roleList  = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            AllRoles = new SelectList(roleList);

            var currentRoles = (await _userManager.GetRolesAsync(user)).ToArray();
            var deleteRoles = currentRoles.Where(r => !RoleNames.Contains(r));
            var addRoles = RoleNames.Where(r => !currentRoles.Contains(r));

            var deleteResult = await _userManager.RemoveFromRolesAsync(user,deleteRoles);
            if (!deleteResult.Succeeded)
            {
                deleteResult.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(string.Empty,error.Description);
                });
                return Page();
            }

            var addResult = await _userManager.AddToRolesAsync(user,addRoles);
            if (!addResult.Succeeded)
            {
                addResult.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(string.Empty,error.Description);
                });
                return Page();
            }
            
            StatusMessage =$"{user.UserName}'s roles is updated!";
            return RedirectToPage("./Index");
        }
    }
}