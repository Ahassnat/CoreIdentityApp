using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController: ControllerBase
    {
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public IActionResult GetUsersWithRoles()
        {
            return Ok("Only Admin can see this");
        }


        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForMedrations")]
        public IActionResult GetPhotosForMedrations()
        {
            return Ok("Admins or Medratores Can see This ");
        }
    }
}