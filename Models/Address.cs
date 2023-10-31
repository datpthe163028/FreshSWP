using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string UserId { get; set; } = null!;
        [Required(ErrorMessage = "Không được để rỗng")]
        public string Province { get; set; } = null!;
        [Required(ErrorMessage = "Không được để rỗng")]
        public string Town { get; set; } = null!;
        [Required(ErrorMessage = "Không được để rỗng")]
        public string District { get; set; } = null!;
        [Required(ErrorMessage = "Không được để rỗng")]
        public string SpecificAdd { get; set; } = null!;    

    }
}
