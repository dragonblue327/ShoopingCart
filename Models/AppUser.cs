using Microsoft.AspNetCore.Identity;

namespace Babelon.Models
{
    public class AppUser :IdentityUser
    {
        public string Occupation { get; set; }
    }
}
