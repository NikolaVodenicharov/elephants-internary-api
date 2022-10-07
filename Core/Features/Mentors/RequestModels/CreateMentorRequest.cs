namespace Core.Features.Mentors.RequestModels
{
    public class CreateMentorRequest
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public IEnumerable<Guid> SpecialityIds { get; set; }

        public CreateMentorRequest(string displayName, string email, IEnumerable<Guid> specialityIds)
        {
            DisplayName = displayName;
            Email = email;
            SpecialityIds = specialityIds;
        }
    }
}
