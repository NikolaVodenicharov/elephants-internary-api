using Core.Features.Interns.RequestModels;

namespace Core.Features.Interns.Interfaces
{
    public interface IInternValidator
    {
        Task ValidateAndThrowAsync(CreateInternRequest request);

        Task ValidateAndThrowAsync(UpdateInternRequest request);

        Task ValidateAndThrowAsync(AddInternCampaignRequest request);

        Task ValidateAndThrowAsync(UpdateInternCampaignRequest request);

        Task ValidateAndThrowAsync(AddStateRequest request);
    }
}
