using UserService.DTOs;
using System.Threading.Tasks;

namespace UserService.Services
{
    public interface IUserService
    {
        Task<string?> Register(UserRegisterDto dto);
        Task<string?> Login(UserLoginDto dto);
    }
}
