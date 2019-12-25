using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        public AdminController(DataContext context)
        {
            _context = context;

        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            // we will use the LINQ sql Query to Join the name of role from AspNetRoles table to RoleId from AspNetUserRoles Table
            var userList = await (from user in _context.Users orderby user.UserName
                                    select new 
                                    {
                                        Id = user.Id,
                                        UserName = user.UserName,
                                        Roles = (from userRole in user.UserRoles
                                                join role in _context.Roles // _context.Roles means the table of AspNetRoles
                                                on userRole.RoleId
                                                equals role.Id
                                                select role.Name).ToList()
                                    }).ToListAsync();
            return Ok(userList);
        }


        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForMedrations")]
        public IActionResult GetPhotosForMedrations()
        {
            return Ok("Admins or Medratores Can see This ");
        }
    }
}