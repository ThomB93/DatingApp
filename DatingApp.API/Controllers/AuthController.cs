using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController] //Allows controller to infer parameters
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userforRegisterDto) //properties are inferred from the object by ApiController (else use FromBody)
        {
            //validate request
            /* if(!ModelState.IsValid) { //use ModelState if not using ApiController
                return BadRequest(ModelState);
            } */

            //convert username to lowercase
            userforRegisterDto.Username = userforRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userforRegisterDto.Username))
                return BadRequest("Username already exists");

            var userToCreate = new User //can only store username for now
            {
                Username = userforRegisterDto.Username
            };
            var createdUser = await _repo.Register(userToCreate, userforRegisterDto.Password); //create user in database

            return StatusCode(201); //returns status code of "Created at route"
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto) //create seperate Dto for login info
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password); //returns user if correct login info, username to lower since it is stored in lowercase in the DB
            if (userFromRepo == null)
                return Unauthorized(); //if login info is incorrect
 
            var claims = new[] { //saves the claims the user has with the login
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()), //Save the ID of the user
                new Claim(ClaimTypes.Name, userFromRepo.Username) //Save the username of the user
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value)); //get the value of the token (appsettings.json) and create a key from it

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); //hash the key and save it

            var tokenDescriptor = new SecurityTokenDescriptor //create data to store in token
            {
                Subject = new ClaimsIdentity(claims), //create claims identity from user claims
                Expires = DateTime.Now.AddDays(1), //token expires in 24 hours
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler(); //used to create the token

            var token = tokenHandler.CreateToken(tokenDescriptor); //contains JWT token that we want returned, created from token description

            return Ok(new { token = tokenHandler.WriteToken(token) }); //write token into response to client
        }


    }
}