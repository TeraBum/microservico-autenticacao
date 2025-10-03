using UserService.Models;

namespace UserService.Services
{
    public interface ITokenService
    {
        string GenerateToken(string id, string email, string[] roles);

        // Overload prático que aceita diretamente o User
        string GenerateToken(User user);
    }
}
