using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using remarsemanal.Model;
using remarsemanal.Model.usuario;

namespace remarsemanal.Utils
{
    public class JWT
    {
        public async Task<string> CriaTokenJWT(Usuario usuario, JwtSettings _jwtSettings) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new [] {
                    new Claim("login", usuario.login),
                    new Claim("id", usuario.id.ToString()),
                    new Claim("nome", usuario.nome),
                    new Claim("primeiroacesso", usuario.primeiroacesso)
                }),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        public async Task<int> RetornaIDUsuarioDoToken(HttpContext context) {
            var identidade = context.User.Identity as ClaimsIdentity;
            if(identidade != null) {
                IEnumerable<Claim> claims = identidade.Claims;
                int idusuario = Int32.Parse(identidade.FindFirst("id").Value);
                return idusuario;
            } else {
                return 0;
            }
        }
    }
}