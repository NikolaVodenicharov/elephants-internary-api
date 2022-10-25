using Core.Common;
using Core.Common.Pagination;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialties.Entities;

namespace Core.Features.Specialities.Interfaces
{
    public interface ISpecialitiesRepository : IRepositoryBase
    {
        Task<SpecialitySummaryResponse> AddAsync(Speciality speciality);

        Task<IEnumerable<SpecialitySummaryResponse>> GetAllAsync(PaginationRequest? filter = null);

        Task<Speciality?> GetByIdAsync(Guid id);

        Task<bool> ExistsByNameAsync(string name);

        Task<bool> IsNameTakenByOtherAsync(string name, Guid updatedCampaignId);

        Task<ICollection<Speciality>> GetByIdsAsync(IEnumerable<Guid> ids);

        Task<int> GetCountAsync();
    }
}
