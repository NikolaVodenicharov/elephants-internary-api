using Core.Features.Campaigns.ResponseModels;
using Core.Features.Campaigns.Support;
using Core.Features.Interns.ResponseModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Persons.Entities;
using Core.Features.Specialities.Support;

namespace Infrastructure.Features.Persons.Support
{
    internal static class PersonMappingExtension
    {
        public static InternSummaryResponse ToInternSummaryResponse(this Person user)
        {
            var email = user.PersonalEmail;

            if (user.WorkEmail != string.Empty)
            {
                email = user.WorkEmail;
            }

            var internSummaryResponse = new InternSummaryResponse(
                user.Id,
                user.DisplayName,
                email);

            return internSummaryResponse;
        }

        public static InternListingResponse ToInternListingResponse(this Person user)
        {
            var email = user.PersonalEmail;

            if (user.WorkEmail != string.Empty)
            {
                email = user.WorkEmail;
            }

            var internSummaryResponse = new InternListingResponse(
                user.Id,
                user.DisplayName,
                email,
                user.InternCampaigns.Select(ic => ic.CampaignId).ToList());

            return internSummaryResponse;
        }

        public static MentorSummaryResponse ToMentorSummaryResponse(this Person user)
        {
            var internSummaryResponse = new MentorSummaryResponse(
                user.Id,
                user.DisplayName,
                user.WorkEmail,
                user.Specialities.ToSpecialitySummaryResponses());

            return internSummaryResponse;
        }

        public static MentorPaginationResponse ToMentorPaginationResponse(this Person person)
        {
            var internSummaryResponse = new MentorPaginationResponse(
                person.Id,
                person.DisplayName,
                person.WorkEmail,
                person.Specialities.ToSpecialitySummaryResponses(),
                person.Campaigns.ToCampaignSummaries());

            return internSummaryResponse;
        }

        public static MentorDetailsResponse ToMentorDetailsResponse(this Person person)
        {
            var campaigns = person
                .Campaigns
                .Select(c => c.ToCampaignSummary())
                .ToList();

            var specialties = person
                .Specialities
                .Select(s => s.ToSpecialitySummaryResponse())
                .ToList();

            var internSummaryResponse = new MentorDetailsResponse(
                person.Id,
                person.DisplayName,
                person.WorkEmail,
                campaigns,
                specialties);

            return internSummaryResponse;
        }
    }
}
