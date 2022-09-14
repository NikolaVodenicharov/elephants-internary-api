using Core.Features.Specialities.ResponseModels;

namespace Core.Features.Mentors.ResponseModels
{
    public class MentorSummaryResponse
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public ICollection<SpecialitySummaryResponse> Specialities { get; set; }

        public MentorSummaryResponse(Guid id, string firstName, string lastName, string email, 
            ICollection<SpecialitySummaryResponse> specialities)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Specialities = specialities;
        }
    }
}
