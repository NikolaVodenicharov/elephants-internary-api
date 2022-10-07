namespace Core.Features.Users.Entities
{
    public class Role 
    {
        public RoleEnum RoleId { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}