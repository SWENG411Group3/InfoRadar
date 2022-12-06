using InformationRadarCore.Models;

namespace InformationRadarCore.Services
{
    public interface IAuthService
    {
        Task<bool> IsAdmin();
        Task<ApplicationUser?> GetUser();
    }
}
