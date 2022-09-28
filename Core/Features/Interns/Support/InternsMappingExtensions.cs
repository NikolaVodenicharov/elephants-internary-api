using Core.Features.Campaigns.Support;
using Core.Features.Interns.Entities;
using Core.Features.Interns.ResponseModels;
using Core.Features.Specialities.Support;

namespace Core.Features.Interns.Support
{
    public static class InternsMappingExtensions
    {
        public static InternSummaryResponse ToInternSummaryResponse(this Intern intern)
        { 
            var internSummaryResponse = new InternSummaryResponse(
                intern.Id,
                intern.FirstName,
                intern.LastName,
                intern.PersonalEmail);

            return internSummaryResponse;
        }

        public static StateResponse ToStateResponse(this State state)
        {
            var stateResponse = new StateResponse(
                state.StatusId.ToString(),
                state.Justification,
                state.Created);

            return stateResponse;
        }

        public static IEnumerable<StateResponse> ToInternStateResponses(this IEnumerable<State> states)
        {
            var internStateResponses = states.Select(s => s.ToStateResponse());

            return internStateResponses;
        }

        public static InternCampaignSummaryResponse ToInternCampaignResponse(this InternCampaign internCampaign)
        {
            var internCapmpaignResponse = new InternCampaignSummaryResponse(
                internCampaign.Campaign.ToCampaignSummary(),
                internCampaign.Speciality.ToSpecialitySummaryResponse(),
                internCampaign
                    .States
                    .First()
                    .ToStateResponse());

            return internCapmpaignResponse;
        }

        public static StatusResponse ToStatusResponse(this Status status)
        {
            var statusResponse = new StatusResponse(
                (int)status.StatusId,
                status.Name);

            return statusResponse;
        }
    }
}
