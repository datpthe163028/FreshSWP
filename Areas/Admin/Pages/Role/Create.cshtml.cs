using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Admin.Role
{
	[Authorize(Roles = "Admin")]

	public class CreateModel : RolePageModel
    {


        public CreateModel(RoleManager<IdentityRole> roleManager, ShopContext shopContext) : base(roleManager, shopContext)
        {
        }

        public class InputModel
		{
			[Display(Name = "Tên của role")]
			[Required(ErrorMessage = "Phải nhập {0}")]
			[StringLength(230, MinimumLength = 3, ErrorMessage = "{0} phải dài {2} đến {1} kí tự")]
			public string Name { get; set; }
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var newRole = new IdentityRole(Input.Name);
			var result = await _roleManager.CreateAsync(newRole);

			if (result.Succeeded)
			{

				StatusMessage = $"Bạn vừa tạo role mới: {Input.Name}";
				return RedirectToPage("./Index");
			}
			else
			{
				result.Errors.ToList().ForEach(error =>
				{
					ModelState.AddModelError(string.Empty, error.Description);
				});
			}

			return Page();
		}
	}
}
