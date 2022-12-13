using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WingtipToys.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace WingtipToys.Admin.User
{
    [Authorize(Roles="admin")] 
    public class IndexModel : PageModel
    {
        const int USER_PER_PAGE = 10;
        private readonly UserManager<AppUser> _userManager;

        [TempData]
        public string StatusMessage { get; set; }

        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet=true)]
        public int currentPage { get; set; }
        public int countPages { get; set; }

        public class UserAndRoles : AppUser
        {
            public string RoleNames { get; set; }
        }
        public List<UserAndRoles> users { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var qr = _userManager.Users.OrderBy(u => u.UserName);
            int totalUsers= await qr.CountAsync();
            countPages = (int)Math.Ceiling((double)totalUsers/USER_PER_PAGE);

            if (currentPage == 0) currentPage = 1; 
            if (currentPage>countPages) currentPage = countPages;

            var qr2 = qr.Skip((currentPage-1)*USER_PER_PAGE)
                        .Take(USER_PER_PAGE)
                        .Select(u => new UserAndRoles(){
                            Id = u.Id,
                            UserName = u.UserName
                        });

            users = await qr2.ToListAsync();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(",",roles);
            }
            return Page();
        }
    }
}
