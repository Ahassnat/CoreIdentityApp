using DatingApp.API.Data;
using DatingApp.API.Dots;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;
        public AdminController(DataContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;

        }



        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            // we will use the LINQ sql Query to Join the name of role from AspNetRoles table to RoleId from AspNetUserRoles Table
            var userList = await (from user in _context.Users
                                  orderby user.UserName
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


        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            // which Roles the user belongs to
            var userRoles = await _userManager.GetRolesAsync(user);
            // which Roles has been selected
            var selectedRoles = roleEditDto.RoleNames;
            // user not exist or exist  Roles
            // user remove from all Roles
            //selectedRoles = selectedRoles != null ? selectedRoles : new string[] {};
            selectedRoles = selectedRoles ?? new string[] { }; // if its not Null(has Role may be "admin","member", etc) use the selectedRoles else use new string[] {}
            // add the user and the role except the existing role 
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded)
                return BadRequest("Failed to add to role");

            // when we need to remove a Role have been selected from the list//
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded)
                return BadRequest("Failed to Remove the Roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }



        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForMedrations")]
        public IActionResult GetPhotosForMedrations()
        {
            return Ok("Admins or Medratores Can see This ");
        }
    }
}