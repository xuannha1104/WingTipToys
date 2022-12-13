using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WingtipToys.Models;
using Microsoft.AspNetCore.Authorization;

namespace WingtipToys.Admin.Role
{
    [Authorize(Roles="admin")] 
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> _roleManager, ProductContext _productContext) : base(_roleManager, _productContext)
        {
        }
        public List<IdentityRole> roles { get; set; }
        public async Task<IActionResult> OnGet()
        {
            roles = await roleManager.Roles.OrderByDescending(r => r.Name).ToListAsync();
            return Page();
        }
    }
}
