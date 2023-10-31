using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Admin.User
{
	[Authorize(Roles = "Admin")]

	public class IndexModel : PageModel
	{
		private readonly UserManager<IdentityUser> _userManager;
		public IndexModel(UserManager<IdentityUser> userManager)
		{
			_userManager = userManager;
		}
		public List<UserAndRole> users { get; set; }

		//public const int ITEMS_PER_PAGE = 10;
		//[BindProperty(SupportsGet =true,Name ="p")]
		//public int currentPage { get;set; }

		//public int countPages { get; set; }


		[TempData]
		public string StatusMessage { get; set; }

		public class UserAndRole : IdentityUser
		{
			public string RoleNames { get; set; }

		}
		public async Task OnGet()
        {
            users = await _userManager.Users.OrderBy(u => u.UserName)
    .Select(u => new UserAndRole
    {
        Id = u.Id,
        UserName = u.UserName
    })
    .ToListAsync();

            foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);
				user.RoleNames =  string.Join(",", roles);
			}

			//var qr = _userManager.Users.OrderBy(u => u.UserName);

			//int totalUsers = await qr.CountAsync();
			//countPages = (int)Math.Ceiling((double)totalUsers / ITEMS_PER_PAGE);
			//if(currentPage<1)
			//	currentPage = 1;
			//if(currentPage>countPages)
			//	currentPage=countPages;

			//var qr1 = qr.Skip((currentPage-1)*ITEMS_PER_PAGE)
			//	.Take(ITEMS_PER_PAGE);

			//users = await qr1.ToListAsync();

		}

		public void OnPost() =>RedirectToPage();
    }
}
