using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using DemoJWT.Interfaces;
using DemoJWT.Entities;
using Microsoft.AspNetCore.Http;

namespace DemoJWT.Services
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly TokenManagement _tokenManagement;
        
        public TokenAuthenticationService(IOptions<TokenManagement> tokenManagement)
        {
            _tokenManagement = tokenManagement.Value;
        }
        public bool IsAuthenticated(Usuario user, out string token)
        {
            var utcNow = DateTime.UtcNow;

            var claims = new Claim[]
            {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                notBefore: utcNow.AddMinutes(-240),
                expires: DateTime.UtcNow.AddMinutes(_tokenManagement.AccessExpiration),
                issuer: _tokenManagement.Issuer,
                audience: _tokenManagement.Issuer
                );

            token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return true;
        }
    }
}
