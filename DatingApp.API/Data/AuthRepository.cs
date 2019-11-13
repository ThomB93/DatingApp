using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username); //returns null if user cant be found
            if(user == null) {
                return null;
            }

            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) {
                return null; //user entered a wrong password
            }

            return user; //username and password was correct
        }
        
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) 
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); //get computed hash using password from user
                for(int i = 0; i < computedHash.Length; i++) { 
                    if(computedHash[i] != passwordHash[i]) return false; //loop trough password hash byte array to compare with entered password hash
                }
            }
            return true; //correct password
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt); //out keyword will pass a reference to the parameter as a the value 

            user.PasswordHash = passwordHash; //save the generated hash on salt on the user that is registering
            user.PasswordSalt = passwordSalt; //password salt is linked to both the password and username

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) //generate a salt key to unlock password hash)
            {
                passwordSalt = hmac.Key; // set to random key that is generated
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // create a hash from the password
            }

        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.Username == username)) //check whether username exists in database
                return true;

            return false;
        }
    }
}