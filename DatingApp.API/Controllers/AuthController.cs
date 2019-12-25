using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dots;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper,
      

        UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _config = config;
            _repo = repo;

        }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
    {

        userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
        if (await _repo.UserExists(userForRegisterDto.Username))
            return BadRequest("UserName is Already Exist");

        // var userToCreate = new User
        // {
        //     Username = userForRegisterDto.Username
        // };
        var userToCreate = _mapper.Map<User>(userForRegisterDto); // user Maaper here to map data to User Model After Registration

        var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

        var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);
        return CreatedAtRoute("GetUser", new { Controller = "users", id = createdUser.Id }, userToReturn); //we used CreatedAtRoute() function to return the user info in postman test >>
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
    {
        var user = await _userManager.FindByNameAsync(userForLoginDto.Username);

        var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

        if (result.Succeeded)
        {
            var appUser = await _userManager.Users.Include(p => p.Photos)
                    .FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());

            var userToReturn = _mapper.Map<UserForListDto>(appUser);

            return Ok(new
        {
            token = GenerateJwtToken(appUser),
            user = userToReturn
        });
       }

        return Unauthorized();
    }

    private string GenerateJwtToken(User user)
    {
        //Token By JWT

        var Claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

        var Key = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(_config.GetSection("AppSettings:Token").Value));

        var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(Claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds

        };


        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);


        //ممكن نختصر الكودين اللي فوق بواحد 
        //var token= new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }



}
}