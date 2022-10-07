using Core.Features.Mentors.Entities;

namespace Core.Features.Users.Entities
{
    public class User 
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        
        public RoleEnum RoleId { get; set; }
        public Role Role { get; set; }
        
        public Guid? MentorId { get; set; }
        public Mentor Mentor { get; set; }
    }
}