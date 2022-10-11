using Core.Features.Specialities.ResponseModels;
using Core.Features.Campaigns.ResponseModels;

namespace Core.Features.Mentors.ResponseModels
{
    public class MentorDetailsResponse
    {
        public Guid Id { get; set; }
        
        public string DisplayName { get; set; }

        public string Email { get; set; }

        public ICollection<SpecialitySummaryResponse> Specialities { get; set; }

        public IEnumerable<CampaignSummaryResponse> Campaigns { get; set; }

        public MentorDetailsResponse(Guid id, string displayName, string email, 
            ICollection<SpecialitySummaryResponse> specialities,
            IEnumerable<CampaignSummaryResponse> campaigns)
        {
            Id = id;
            DisplayName = displayName;
            Email = email;
            Specialities = specialities;
            Campaigns = campaigns;
        }
    }
}
