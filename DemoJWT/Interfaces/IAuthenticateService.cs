using DemoJWT.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DemoJWT.Interfaces
{
    public interface IAuthenticateService
    {
        Task<string> IsAuthenticated(Usuario user);

        Task<List<Claim>> GetValidClaims(Usuario user);
    }
}
