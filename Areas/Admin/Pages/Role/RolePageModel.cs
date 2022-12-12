using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using WingtipToys.Models;

namespace WingtipToys.Admin.Role
{
    public class RolePageModel : PageModel
    {
        protected RoleManager<IdentityRole> roleManager;

        protected ProductContext productContext;
        
        [TempData]
        public string StatusMessage { get; set; }
        public RolePageModel(RoleManager<IdentityRole> _roleManager,ProductContext _productContext)
        {
            roleManager = _roleManager;
            productContext = _productContext;
        }
    }
}