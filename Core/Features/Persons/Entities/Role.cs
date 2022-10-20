namespace Core.Features.Persons.Entities
{
    public class Role
    {
        public RoleId RoleId { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<PersonRole> PersonRoles { get; set; } = null!;
    }
}