using Core.Common.Pagination;
using Core.Features.Campaigns.Support;
using Core.Features.Interns.Entities;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.ResponseModels;
using Core.Features.Interns.Support;
using Core.Features.Specialities.Support;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Interns
{
    public class InternsRepository : IInternsRepository
    {
        private readonly InternaryContext context;

        public InternsRepository(InternaryContext context)
        {
            this.context = context;
        }

        public async Task<InternSummaryResponse> AddAsync(Intern intern)
        {
            await context
                .Interns
                .AddAsync(intern);

            await context.SaveChangesAsync();
            
            return intern.ToInternSummaryResponse();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var exist = await context
                .Interns
                .AnyAsync(i => i.PersonalEmail == email);

            return exist;
        }

        public Task<Intern?> GetByIdAsync(Guid Id)
        {
            var intern = context
                .Interns
                .FirstOrDefaultAsync(i => i.Id == Id);

            return intern;
        }

        public async Task<InternDetailsResponse?> GetDetailsByIdAsync(Guid id)
        {
            var internDetailsResponse = await context
                .Interns
                .Where(i => i.Id == id)
                .Select(i => new InternDetailsResponse(
                    i.Id,
                    i.FirstName,
                    i.LastName,
                    i.PersonalEmail,
                    i.InternCampaigns
                        .Select(internCampaign => new InternCampaignSummaryResponse(
                            internCampaign
                                .Campaign
                                .ToCampaignSummary(),
                            internCampaign
                                .Speciality
                                .ToSpecialitySummaryResponse(),
                            internCampaign
                                .States
                                .OrderByDescending(s => s.Created)
                                .First()
                                .ToStateResponse()))
                        .ToList()))
                .FirstOrDefaultAsync();

            return internDetailsResponse;
        }

        public async Task<InternCampaign?> GetInternCampaignByIdsAsync(Guid internId, Guid campaignId)
        {
            var internIntersection = await context
                .InternCampaigns
                .Include(ic => ic.Intern)
                .Include(ic => ic.Campaign)
                .Include(ic => ic.Speciality)
                .Include(ic => ic.States)
                .FirstOrDefaultAsync(i =>
                    i.InternId == internId &&
                    i.CampaignId == campaignId);

            return internIntersection;
        }

        public async Task<PaginationResponse<InternSummaryResponse>> GetAllAsync(PaginationRequest paginationRequest)
        {
            var elementsCount = await context
                .Interns
                .CountAsync();

            if (elementsCount == 0)
            {
                var zeroElementResult = new PaginationResponse<InternSummaryResponse>(
                new List<InternSummaryResponse>(),
                paginationRequest.PageNum.Value,
                PaginationConstants.DefaultPageCount);

                return zeroElementResult;
            }

            var pageCount = CalculatePageCount(paginationRequest, elementsCount);

            var skip = CalculateSkipCount(paginationRequest);

            var interns = await context
                .Interns
                .AsNoTracking()
                .Skip(skip)
                .Take(paginationRequest.PageSize.Value)
                .ToListAsync();

            var internSummaryResponses = interns
                .Select(i => i.ToInternSummaryResponse())
                .ToList();

            var paginationResponse = new PaginationResponse<InternSummaryResponse>(
                internSummaryResponses, 
                paginationRequest.PageNum.Value, 
                pageCount);

            return paginationResponse;
        }

        public async Task<PaginationResponse<InternByCampaignSummaryResponse>> GetAllByCampaignIdAsync(PaginationRequest paginationRequest, Guid campaignId)
        {
            var elementsCount = await context
                .InternCampaigns
                .Where(ic => ic.CampaignId == campaignId)
                .CountAsync();

            if (elementsCount == 0)
            {
                var zeroElementResult = new PaginationResponse<InternByCampaignSummaryResponse>(
                new List<InternByCampaignSummaryResponse>(),
                paginationRequest.PageNum.Value,
                PaginationConstants.DefaultPageCount);

                return zeroElementResult;
            }

            int pageCount = CalculatePageCount(paginationRequest, elementsCount);

            int skip = CalculateSkipCount(paginationRequest);

            var internsByCampaignSummaryResponse = await context
                .InternCampaigns
                .AsNoTracking()
                .Where(ic => ic.CampaignId == campaignId)
                .Select(ic => new InternByCampaignSummaryResponse(
                    ic
                        .Intern
                        .ToInternSummaryResponse(),
                    ic
                        .Speciality
                        .ToSpecialitySummaryResponse(),
                    ic
                        .States
                        .OrderByDescending(s => s.Created)
                        .First()
                        .ToStateResponse()))
                .Skip(skip)
                .Take(paginationRequest.PageSize.Value)
                .ToListAsync();

            var paginationResponse = new PaginationResponse<InternByCampaignSummaryResponse>(
                internsByCampaignSummaryResponse,
                paginationRequest.PageNum.Value,
                pageCount);

            return paginationResponse;
        }

        public async Task<IEnumerable<StatusResponse>> GetAllStatusAsync()
        {
            var statusResponseCollection = await context
                .Status
                .AsNoTracking()
                .Select(i => i.ToStatusResponse())
                .ToListAsync();

            return statusResponseCollection;
        }

        public async Task SaveTrackingChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        private static int CalculatePageCount(PaginationRequest paginationRequest, int elementsCount)
        {
            return (int)Math.Ceiling((double)elementsCount / paginationRequest.PageSize.Value);
        }

        private static int CalculateSkipCount(PaginationRequest paginationRequest)
        {
            return (paginationRequest.PageNum.Value - 1) * paginationRequest.PageSize.Value;
        }
    }
}
