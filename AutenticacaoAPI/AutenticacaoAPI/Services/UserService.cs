using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AutenticacaoAPI.Entities;
using AutenticacaoAPI.Helpers;

namespace AutenticacaoAPI.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
    }

    public class UserService : IUserService
    {
        private List<User> v_UserList = new List<User>
        {
            new User { Id = 1, Username = "Usuário Teste", Password = "senhateste" }
        };

        private readonly AppSettings AppSettings;

        public UserService(IOptions<AppSettings> p_AppSettings)
        {
            AppSettings = p_AppSettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            var v_User = v_UserList.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (v_User == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AppSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, v_User.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            v_User.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            v_User.Password = null;

            return v_User;
        }

        public IEnumerable<User> GetAll()
        {
            // return users without passwords
            return v_UserList.Select(x => {
                x.Password = null;
                return x;
            });
        }
    }
}