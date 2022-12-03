using Microsoft.AspNetCore.Identity;

namespace InformationRadarCore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsAdmin { get; set; } = false;
        public ICollection<Lighthouse> Lighthouses { get; set; }
    }
}