namespace Core.Features.Mentors.RequestModels
{
    public class CreateMentorRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public IEnumerable<Guid> SpecialityIds { get; set; }

        public CreateMentorRequest(string firstName, string lastName, string email, IEnumerable<Guid> specialityIds)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            SpecialityIds = specialityIds;
        }
    }
}
