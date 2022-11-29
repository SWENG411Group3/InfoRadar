using InformationRadarCore.Data.Migrations;
using InformationRadarCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace InformationRadarCore.Controllers
{

    [Route("api/[controller]")]
    
    [ApiController]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        public AdminController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult IsAdmin()
        {

            //userManager.IsInRoleAsync(User.Identity.)
            return Ok(new
            {
                //Roles = claims.Claims.Where(e => e.Type == ClaimTypes.Role).Select(e => e.Value).ToList(),
                
                //Roles2 = userManager.GetUsersInRoleAsync("ADMIN"),
                IsAdmin = User.IsInRole("Admin"),
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Promote()
        {
            
            return Ok();
        }
    }
}
