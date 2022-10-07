using Core.Common;
using Core.Common.Pagination;
using Core.Features.Campaigns.Support;
using Core.Features.Interns.Entities;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.ResponseModels;
using Core.Features.Interns.Support;
using Core.Features.Specialities.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Features.Interns
{
    public class InternsRepository : IInternsRepository
    {
        private readonly InternaryContext context;
        private readonly ILogger<InternsRepository> internsRepositoryLogger;

        public InternsRepository(InternaryContext context, ILogger<InternsRepository> internsRepositoryLogger)
        {
            this.context = context;
            this.internsRepositoryLogger = internsRepositoryLogger;
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
            Guard.EnsureNotNullPagination(paginationRequest.PageNum, paginationRequest.PageSize, 
                internsRepositoryLogger, nameof(InternsRepository));

            var elementsCount = await context
                .Interns
                .CountAsync();

            if (elementsCount == 0)
            {
                if (paginationRequest.PageNum > PaginationConstants.DefaultPageCount)
                {
                    internsRepositoryLogger.LogErrorAndThrowExceptionPageCount(nameof(InternsRepository),
                        PaginationConstants.DefaultPageCount, paginationRequest.PageNum.Value);
                }

                var zeroElementResult = new PaginationResponse<InternSummaryResponse>(
                    new List<InternSummaryResponse>(),
                    paginationRequest.PageNum.Value,
                    PaginationConstants.DefaultPageCount);

                return zeroElementResult;
            }

            var pageCount = CalculatePageCount(paginationRequest.PageSize.Value, elementsCount);

            if (paginationRequest.PageNum > pageCount)
            {
                internsRepositoryLogger.LogErrorAndThrowExceptionPageCount(nameof(InternsRepository),
                    pageCount, paginationRequest.PageNum.Value);
            }

            var skip = CalculateSkipCount(paginationRequest.PageNum.Value, paginationRequest.PageSize.Value);

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
            Guard.EnsureNotNullPagination(paginationRequest.PageNum, paginationRequest.PageSize,
                internsRepositoryLogger, nameof(InternsRepository));

            var elementsCount = await context
                .InternCampaigns
                .Where(ic => ic.CampaignId == campaignId)
                .CountAsync();

            if (elementsCount == 0)
            {
                if (paginationRequest.PageNum > PaginationConstants.DefaultPageCount)
                {
                    internsRepositoryLogger.LogErrorAndThrowExceptionPageCount(nameof(InternsRepository),
                        PaginationConstants.DefaultPageCount, paginationRequest.PageNum.Value);
                }

                var zeroElementResult = new PaginationResponse<InternByCampaignSummaryResponse>(
                new List<InternByCampaignSummaryResponse>(),
                paginationRequest.PageNum.Value,
                PaginationConstants.DefaultPageCount);

                return zeroElementResult;
            }

            int pageCount = CalculatePageCount(paginationRequest.PageSize.Value, elementsCount);

            if (paginationRequest.PageNum > pageCount)
            {
                internsRepositoryLogger.LogErrorAndThrowExceptionPageCount(nameof(InternsRepository),
                    pageCount, paginationRequest.PageNum.Value);
            }

            int skip = CalculateSkipCount(paginationRequest.PageNum.Value, paginationRequest.PageSize.Value);

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

        private static int CalculatePageCount(int pageSize, int elementsCount)
        {
            return (int)Math.Ceiling((double)elementsCount / pageSize);
        }

        private static int CalculateSkipCount(int pageNum, int pageSize)
        {
            return (pageNum - 1) * pageSize;
        }
    }
}
