using care_core.dto.auth;

namespace care_core.security.services
{
    public interface ITokenService
    {
        TokenDto Authenticate(string email, string password);
    }
}