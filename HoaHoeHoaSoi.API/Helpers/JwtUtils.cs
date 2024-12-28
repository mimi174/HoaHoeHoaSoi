using HoaHoeHoaSoi.API.Models;
using HoaHoeHoaSoi.API.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HoaHoeHoaSoi.API.Helpers
{
    public static class JwtUtils
    {
        private static readonly string SECRET_KEY = "HHS_d5a87b23d700bf97350da1ecb14438718a96318984d858dea51e31c245a0c5d75ae7b8a4c3bc8eed444c5fddad76a48cd70e7b0a66bc56337bd9ab8b291ac1b87bb039f5c2d64b5b53a55e187a95bc2d872437528aba7a09bb4bbff9be46206f197706e8684528e86fd496a80766890d3c9396a3b2f3108f1042db46290042670d55e6ccab19219cb8af6e911fc410bdcc1e2fe8b2538b42fe6593bcd5bfba3357fc0a516a56b5adfdf82ca7a4aefc45932dc9221a34cdae00c454f7ab86e715ca52ab68662fd071dcadf969f0c9f27008c2d21151718b74439262029a76b26326bbe4fec9149a9fe1b2df7b1251a0e2ad96d397b4593f8c352a40093b585510";
        private static readonly string AUDIENCE = "HoaHoeHoaSoi_AUD";
        private static readonly string ISSUER = "HoaHoeHoaSoi_ISS";
        private static readonly byte[] KEY = Encoding.ASCII.GetBytes(SECRET_KEY);

        public static readonly TokenValidationParameters TOKEN_VALIDATION_PARAMETERS = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(KEY),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = AUDIENCE,
            ValidIssuer = ISSUER,
            ClockSkew = TimeSpan.Zero
        };

        public static int ValidateToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(token, TOKEN_VALIDATION_PARAMETERS, out SecurityToken validatedToken);
            JwtSecurityToken validatedJWT = (JwtSecurityToken)validatedToken;

            var id = validatedJWT.Claims.FirstOrDefault(c => c.Type == "Id");
            return id != null ? Int32.Parse(id.Value) : 0;
        }

        public static string GenerateToken(UserInfo user)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            List<Claim> claims = new List<Claim>()
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Name", user.Name)
            };

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(KEY), SecurityAlgorithms.HmacSha256Signature),
                Issuer = ISSUER,
                Audience = AUDIENCE
            };

            var jwtToken = handler.CreateToken(securityTokenDescriptor);
            return handler.WriteToken(jwtToken);
        }
    }
}
