using Core.Features.Specialities.ResponseModels;
using Core.Features.Campaigns.ResponseModels;

namespace Core.Features.Mentors.ResponseModels
{
    public class MentorSummaryResponse
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public ICollection<SpecialitySummaryResponse> Specialities { get; set; }

        public IEnumerable<CampaignSummaryResponse> Campaigns { get; set; }

        public MentorSummaryResponse(Guid id, string firstName, string lastName, string email, 
            ICollection<SpecialitySummaryResponse> specialities,
            IEnumerable<CampaignSummaryResponse> campaigns)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Specialities = specialities;
            Campaigns = campaigns;
        }
    }
}
