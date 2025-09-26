namespace UserService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string SenhaHash { get; set; }
        public string Role { get; set; }
    }
}