using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using DemoJWT.Interfaces;
using DemoJWT.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DemoJWT.Services
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly TokenManagement _tokenManagement;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<Rol> _roleManager;

        public TokenAuthenticationService(IOptions<TokenManagement> tokenManagement,
                                            UserManager<Usuario> userManager,
                                            RoleManager<Rol> roleManager)
        {
            _tokenManagement = tokenManagement.Value;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> IsAuthenticated(Usuario user)
        {
            var utcNow = DateTime.UtcNow;

            var claims = await GetValidClaims(user);

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

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return await Task.FromResult(token);
        }

        public async Task<List<Claim>> GetValidClaims(Usuario user)
        {
            var utcNow = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString())
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.AddRange(userClaims);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            return claims;
        }
    }

}
