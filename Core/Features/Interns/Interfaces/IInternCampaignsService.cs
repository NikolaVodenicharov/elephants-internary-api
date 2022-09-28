using Core.Features.Interns.Entities;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;

namespace Core.Features.Interns.Interfaces
{
    public interface IInternCampaignsService
    {
        Task<StateResponse> AddStateAsync(AddStateRequest createStateRequest);

        Task<InternCampaignSummaryResponse> AddInternCampaignAsync(AddInternCampaignRequest addInternCampaignRequest);

        Task<InternCampaignSummaryResponse> UpdateInternCampaignAsync(UpdateInternCampaignRequest updateInternCampaignRequest);

        Task<InternCampaign> CreateInternCampaignAsync(Guid campaignId, Guid specialityId, string justificaton);

        Task<IEnumerable<StatusResponse>> GetAllStatusAsync();
    }
}
