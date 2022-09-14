namespace Core.Features.Mentors.RequestModels
{
    public class UpdateMentorRequest
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public IEnumerable<Guid> SpecialityIds { get; set; }

        public UpdateMentorRequest(Guid id, string firstName, string lastName, string email, IEnumerable<Guid> specialityIds)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            SpecialityIds = specialityIds;
        }
    }
}
