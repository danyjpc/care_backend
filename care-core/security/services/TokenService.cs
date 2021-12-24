using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using care_core.dto.auth;
using care_core.model;
using care_core.repository.interfaces;
using care_core.security.settings;
using care_core.util;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace care_core.security.services
{
    public class TokenService : ITokenService
    {
        private readonly IAdmUser admUserRepository;
        private readonly IAdmPerson admPersonRepository;

        private readonly AppSettings _appSettings;

        public TokenService(
            IOptions<AppSettings> appSettings,
            IAdmUser _userRepository,
            IAdmPerson _personRepository
        )
        {
            _appSettings = appSettings.Value;
            admUserRepository = _userRepository;
            admPersonRepository = _personRepository;
        }
        
        
        public TokenDto Authenticate(string email, string password)
        {
            TokenDto token = new TokenDto();

            AdmUser admUser = admUserRepository.findByCredentials(email,password);

            // return null if user not found
            if (admUser == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, admUser.role.description),
                    new Claim("role_id", admUser.role.typology_id.ToString()),
                    new Claim("person_id", admUser.person.person_id.ToString()),
                    new Claim("user_id", admUser.user_id.ToString()),
                    new Claim("name", admUser.person.first_name + " " + admUser.person.last_name)
                }),

                Expires = DateTime.UtcNow.AddDays(CareConstants.TOKEN_DAYS),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenCreated = tokenHandler.CreateToken(tokenDescriptor);
            token.token = tokenHandler.WriteToken(tokenCreated);
            token.expire = CsnFunctions.ConvertToUnixTime((DateTime) tokenDescriptor.Expires);

            return token.WithoutPassword();
        }
        
    }
}