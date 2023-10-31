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

	public class DeleteModel : RolePageModel
    {
		public DeleteModel(RoleManager<IdentityRole> roleManager, ShopContext shopContext) : base(roleManager, shopContext)
		{
		}

		public IdentityRole role { get; set; }

		public async Task<IActionResult> OnGet(string roleid)
		{
			if(roleid == null) return NotFound("Không tìm thấy role");

			role = await _roleManager.FindByIdAsync(roleid);

			if(role == null)
			{
                return NotFound("Không tìm thấy role");
            }
            return Page();
        }

		public async Task<IActionResult> OnPostAsync(string roleid)
		{
            if (roleid == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(roleid);
            if (roleid == null) return NotFound("Không tìm thấy role");


            
		
			var result=await _roleManager.DeleteAsync(role);



			if (result.Succeeded)
			{

				StatusMessage = $"Bạn vừa xóa role: {role.Name}";
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
