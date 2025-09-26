namespace UserService.Models.DTOs
{
    // DTO para registro de usuário
    public class UserRegisterRequest
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Role { get; set; }
    }

    // DTO para login
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }

    // DTO para resposta (não inclui senha)
    public class UserResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    // DTO para atualização do usuário
    public class UserUpdateRequest
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string? Senha { get; set; }  // senha opcional na atualização
        public string Role { get; set; }
    }
}
