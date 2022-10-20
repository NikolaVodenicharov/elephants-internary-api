namespace Core.Features.Persons.Entities
{
    public class PersonRole
    {
        public Guid PersonId { get; set; }
        public Person Person { get; set; } = null!;
        public RoleId RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
