namespace UserService.Configurations
{
    public static class UserRoles
    {
        public const string User = "User";
        public const string Admin = "Administrador";
        public const string Manager = "Manager";

        public static readonly string[] All = { User, Admin, Manager };
    }
}
