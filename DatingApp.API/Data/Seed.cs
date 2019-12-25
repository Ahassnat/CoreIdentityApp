using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly UserManager<User> _userManager;
        public Seed(UserManager<User> userManager)
        {
            _userManager = userManager;


        }

        public void SeedUsers()
        {
            if (!_userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");// read from the json file 
                var users = JsonConvert.DeserializeObject<List<User>>(userData);//to save all user as Object
                foreach (var user in users)
                {
                   _userManager.CreateAsync(user, "password").Wait(); 
                }
                
            }

        }
    }
}