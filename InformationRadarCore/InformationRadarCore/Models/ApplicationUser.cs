using Microsoft.AspNetCore.Identity;

namespace InformationRadarCore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Lighthouse> Lighthouses { get; set; }
    }
}