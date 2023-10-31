using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;

namespace Project.Admin.Role
{
	[Authorize(Roles = "Admin")]

	public class IndexModel : RolePageModel
	{
		public IndexModel(RoleManager<IdentityRole> roleManager, ShopContext shopContext) : base(roleManager, shopContext)
		{
		}
		public List<IdentityRole> roles { get; set; }
		public async Task OnGet()
        {
			roles = await _roleManager.Roles.ToListAsync();
        }

		public void OnPost() =>RedirectToPage();
    }
}
