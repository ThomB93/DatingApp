using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterUserDto registerUser)
        {
            if(await UserExists(registerUser.Username)) {
                return BadRequest("Username is taken");
            }

            using var hmac = new HMACSHA512();

            var appUser = new AppUser
            {
                UserName = registerUser.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerUser.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(appUser);
            await _context.SaveChangesAsync();

            return appUser;
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
