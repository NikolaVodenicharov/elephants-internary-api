using Core.Features.Mentors.RequestModels;

namespace Core.Features.Mentors.Interfaces
{
    public interface IMentorValidator
    {
        Task ValidateAndThrowAsync(CreateMentorRequest request);

        Task ValidateAndThrowAsync(UpdateMentorRequest request);
    }
}
