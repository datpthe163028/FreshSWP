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

	public class UpdateModel : RolePageModel
    {

        public UpdateModel(RoleManager<IdentityRole> roleManager, ShopContext shopContext) : base(roleManager, shopContext)
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
		public IdentityRole role { get; set; }

		public async Task<IActionResult> OnGet(string roleid)
		{
			if(roleid == null) return NotFound("Không tìm thấy role");

			 role = await _roleManager.FindByIdAsync(roleid);

			if(role != null)
			{
				Input = new InputModel
				{
					Name = role.Name
				};
				return Page();
			}
            return NotFound("Không tìm thấy role");
        }

		public async Task<IActionResult> OnPostAsync(string roleid)
		{
            if (roleid == null) return NotFound("Không tìm thấy role");

             role = await _roleManager.FindByIdAsync(roleid);
            if (roleid == null) return NotFound("Không tìm thấy role");


            if (!ModelState.IsValid)
			{
				return Page();
			}
			role.Name =Input.Name;
			var result=await _roleManager.UpdateAsync(role);



			if (result.Succeeded)
			{

				StatusMessage = $"Bạn vừa sửa role: {Input.Name}";
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
