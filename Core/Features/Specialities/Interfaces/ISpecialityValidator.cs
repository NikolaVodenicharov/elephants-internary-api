using Core.Features.Specialities.RequestModels;

namespace Core.Features.Specialities.Interfaces
{
    public interface ISpecialityValidator
    {
        Task ValidateAndThrowAsync(CreateSpecialityRequest request);

        Task ValidateAndThrowAsync(UpdateSpecialityRequest request);
    }
}
