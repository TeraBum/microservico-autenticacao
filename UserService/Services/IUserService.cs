using UserService.DTOs;
using UserService.Models;

namespace UserService.Services
{
    public interface IUserService
    {
        string Register(UserRegisterDto dto);
        string Login(UserLoginDto dto);
    }
}