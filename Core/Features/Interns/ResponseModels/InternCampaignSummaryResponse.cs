using Core.Features.Campaigns.ResponseModels;
using Core.Features.Specialities.ResponseModels;

namespace Core.Features.Interns.ResponseModels
{
    public record InternCampaignSummaryResponse(
        CampaignSummaryResponse Campaign, 
        SpecialitySummaryResponse Speciality, 
        StateResponse StateResponse);
}
