using Core.Common.Pagination;
using Core.Features.Specialities.RequestModels;
using Core.Features.Specialities.ResponseModels;

namespace Core.Features.Specialities.Interfaces
{
    public interface ISpecialitiesService
    {
        Task<SpecialitySummaryResponse> CreateAsync(CreateSpecialityRequest createSpeciality);

        Task<SpecialitySummaryResponse> UpdateAsync(UpdateSpecialityRequest updateSpeciality);

        Task<IEnumerable<SpecialitySummaryResponse>> GetAllAsync();

        Task<PaginationResponse<SpecialitySummaryResponse>> GetPaginationAsync(PaginationRequest filter);

        Task<SpecialitySummaryResponse> GetByIdAsync(Guid id);
    }
}
