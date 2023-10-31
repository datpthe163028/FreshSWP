using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data;

namespace Project.Admin.Role
{
	public class RolePageModel :PageModel
	{

		protected readonly RoleManager<IdentityRole> _roleManager;

		protected readonly ShopContext _shopContext;

		[TempData]
		public string StatusMessage { get; set; }

		public RolePageModel(RoleManager<IdentityRole> roleManager, ShopContext shopContext)
		{

			_roleManager = roleManager;
			_shopContext = shopContext;


		}
	}
}
