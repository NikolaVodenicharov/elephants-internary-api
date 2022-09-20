using Core.Common;
using Core.Features.Specialities.ResponseModels;
using Core.Features.Specialties.Entities;

namespace Core.Features.Specialities.Interfaces
{
    public interface ISpecialitiesRepository : IRepositoryBase
    {
        Task<SpecialitySummaryResponse> AddAsync(Speciality specialty);

        Task<IEnumerable<SpecialitySummaryResponse>> GetAllAsync();

        Task<Speciality?> GetByIdAsync(Guid id);

        Task<bool> ExistsByNameAsync(string name);

        Task<bool> IsNameTakenByOtherAsync(string name, Guid updatedCampaignId);

        Task<ICollection<Speciality>> GetByIdsAsync(IEnumerable<Guid> ids);
    }
}
