using UserService.Models;

namespace UserService.Repositories
{
    public interface IUserRepository
    {
        User GetByEmail(string email);
        void Add(User user);
    }
}