using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace clothing_shop.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
    }
}
