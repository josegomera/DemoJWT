using DemoJWT.Entities;

namespace DemoJWT.Interfaces
{
    public interface IAuthenticateService
    {
        bool IsAuthenticated(Usuario user, out string token);
    }
}
