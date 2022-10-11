using Core.Features.Mentors.Entities;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Specialities.Support;
using Core.Features.Campaigns.Support;

namespace Core.Features.Mentors.Support
{
    public static class MentorsMappingExtensions
    {
        public static Mentor ToMentor(this CreateMentorRequest request)
        {
            var mentor = new Mentor()
            {
                DisplayName = request.DisplayName,
                Email = request.Email
            };

            return mentor;
        }

        public static MentorDetailsResponse ToMentorDetailsResponse(this Mentor mentor)
        {
            var response = new MentorDetailsResponse(mentor.Id, mentor.DisplayName, mentor.Email, 
                mentor.Specialities.ToSpecialitySummaryResponses().ToList(),
                mentor.Campaigns.ToCampaignSummaries().ToList());

            return response;
        }

        public static IEnumerable<MentorDetailsResponse> ToMentorDetailsResponses(this IEnumerable<Mentor> mentors)
        {
            var response = mentors.Select(m => m.ToMentorDetailsResponse());

            return response;
        }

        public static MentorSummaryResponse ToMentorSummaryResponse(this Mentor mentor)
        {
            var response = new MentorSummaryResponse(
                mentor.Id, 
                mentor.DisplayName, 
                mentor.Email, 
                mentor.Specialities.ToSpecialitySummaryResponses().ToList());

            return response;
        }
    }
}
