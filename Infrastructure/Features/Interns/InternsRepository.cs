using Core.Common.Pagination;
using Core.Features.Interns.Entities;
using Core.Features.Interns.Interfaces;
using Core.Features.Interns.RequestModels;
using Core.Features.Interns.ResponseModels;
using Core.Features.Interns.Support;
using Core.Features.Persons.Entities;
using Infrastructure.Features.Persons.Support;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Features.Interns
{
    public class InternsRepository : IInternsRepository
    {
        private readonly InternaryContext context;

        public InternsRepository(InternaryContext context)
        {
            this.context = context;
        }

        public async Task<InternSummaryResponse> CreateAsync(CreateInternRepoRequest createInternWithDetailsRequest)
        {
            var person = new Person()
            {
                FirstName = createInternWithDetailsRequest.FirstName,
                LastName = createInternWithDetailsRequest.LastName,
                DisplayName = CreateDisplayName(createInternWithDetailsRequest.FirstName, createInternWithDetailsRequest.LastName),
                PersonalEmail = createInternWithDetailsRequest.Email,
                InternCampaigns = new List<InternCampaign>() { createInternWithDetailsRequest.InternCampaign }
            };

            var personRole =
                new PersonRole()
                {
                    RoleId = RoleId.Intern,
                    Person = person,
                };

            await context
                .PersonRoles
                .AddAsync(personRole);

            await context.SaveChangesAsync();

            return person.ToInternSummaryResponse();
        }

        public async Task<InternCampaignSummaryResponse?> AddInternCampaignAsync(AddInternCampaignRepoRequest addInternCampaignRepoRequest)
        {
            var Person = await context
                .Persons
                .Where(p => p.Id == addInternCampaignRepoRequest.InternId)
                .Where(HasInternRole())
                .Include(p => p.InternCampaigns)
                .FirstOrDefaultAsync();

            if (Person == null)
            {
                return null;
            }

            Person.InternCampaigns.Add(addInternCampaignRepoRequest.InternCampaign);

            await context.SaveChangesAsync();

            return addInternCampaignRepoRequest.InternCampaign.ToInternCampaignResponse();
        }

        public async Task<InternSummaryResponse?> AddIdentityAsync(AddInternIdentityRepoRequest addInternIdentityRepoRequest)
        {
            var person = await context
                .Persons
                .Where(HasInternRole())
                .Where(p => p.Id == addInternIdentityRepoRequest.Id)
                .FirstOrDefaultAsync();

            if (person == null)
            {
                return null;
            }

            person.DisplayName = addInternIdentityRepoRequest.DisplayName;
            person.WorkEmail = addInternIdentityRepoRequest.WorkEmail;

            await context.SaveChangesAsync();

            return person.ToInternSummaryResponse();
        }

        public async Task<InternSummaryResponse?> UpdateAsync(UpdateInternRequest updateInternRequest)
        {
            var person = await context
                .Persons
                .Where(u => u.Id == updateInternRequest.Id)
                .Where(HasInternRole())
                .FirstOrDefaultAsync();

            if (person == null)
            {
                return null;
            }

            UpdateDisplayName(updateInternRequest, person);

            person.FirstName = updateInternRequest.FirstName;
            person.LastName = updateInternRequest.LastName;
            person.PersonalEmail = updateInternRequest.Email;

            await context.SaveChangesAsync();

            return person.ToInternSummaryResponse();
        }

        public async Task<InternCampaignSummaryResponse> UpdateInternCampaignAsync(InternCampaign internCampaign)
        {
            context
                .InternCampaigns
                .Update(internCampaign);

            await context.SaveChangesAsync();

            return internCampaign.ToInternCampaignResponse();
        }

        public async Task<InternSummaryResponse?> GetByIdAsync(Guid Id)
        {
            var person = await context
                .Persons
                .Where(p => p.Id == Id)
                .Where(HasInternRole())
                .FirstOrDefaultAsync();

            return person?.ToInternSummaryResponse();
        }

        public async Task<bool> ExistsByPersonalEmailAsync(string personalEmail)
        {
            var exist = await context
                .Persons
                .AsNoTracking()
                .AnyAsync(i => i.PersonalEmail == personalEmail);

            return exist;
        }

        public async Task<InternDetailsResponse?> GetDetailsByIdAsync(Guid id)
        {
            var person = await context
                .Persons
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Where(HasInternRole())
                    .Include(p => p.InternCampaigns)
                        .ThenInclude(ic => ic.Speciality)
                    .Include(p => p.InternCampaigns)
                        .ThenInclude(ic => ic.Campaign)
                    .Include(p => p.InternCampaigns)
                        .ThenInclude(ic => ic.States)
                .FirstOrDefaultAsync();

            if (person == null)
            {
                return null;
            }

            var internDetailsResponse = new InternDetailsResponse(
                person.Id,
                person.DisplayName,
                person.FirstName,
                person.LastName,
                person.PersonalEmail,
                person.WorkEmail,
                person
                    .InternCampaigns
                    .Select(ic => ic.ToInternCampaignResponse()));

            return internDetailsResponse;
        }
    
        public async Task<InternCampaign?> GetInternCampaignByIdsAsync(Guid internId, Guid campaignId)
        {
            var internCampaign = await context
                .InternCampaigns
                .Where(ic =>
                    ic.PersonId == internId &&
                    ic.CampaignId == campaignId &&
                    ic.Person.PersonRoles.Any(pr => pr.RoleId == RoleId.Intern))
                .Include(ic => ic.Person)
                .Include(ic => ic.Campaign)
                .Include(ic => ic.Speciality)
                .Include(ic => ic.States)
                .FirstOrDefaultAsync();

            return internCampaign;
        }

        public async Task<IEnumerable<InternListingResponse>> GetAllAsync()
        {
            var interns = await context
                .Persons
                .Where(HasInternRole())
                .Include(p => p.InternCampaigns)
                .ToListAsync();

            var internListingResponses = interns
                .Select(i => i.ToInternListingResponse())
                .ToList();

            return internListingResponses;
        }

        public async Task<PaginationResponse<InternListingResponse>> GetPaginationAsync(PaginationRequest paginationRequest)
        {
            int elementsCount = await context
                .Persons
                .Where(HasInternRole())
                .CountAsync();

            if (elementsCount == 0)
            {
                var zeroElementPagination = new PaginationResponse<InternListingResponse>(
                    new List<InternListingResponse>(),
                    paginationRequest!.PageNum!.Value,
                    PaginationConstants.DefaultPageCount);

                return zeroElementPagination;
            }

            var paginationResponse = await CreatePaginationInternListingResponse(paginationRequest, elementsCount);

            return paginationResponse;
        }

        public async Task<PaginationResponse<InternSummaryResponse>> GetPaginationByCampaignIdAsync(PaginationRequest paginationRequest, Guid campaignId)
        {
            var elementsCount = await context
                .InternCampaigns
                .Where(ic =>
                    ic.Person.PersonRoles.Any(pr => pr.RoleId == RoleId.Intern) &&
                    ic.CampaignId == campaignId)
                .CountAsync();

            if (elementsCount == 0)
            {
                var zeroElementResult = new PaginationResponse<InternSummaryResponse>(
                    new List<InternSummaryResponse>(),
                    paginationRequest!.PageNum!.Value,
                    PaginationConstants.DefaultPageCount);

                return zeroElementResult;
            }

            int pageCount = CalculatePageCount(paginationRequest!.PageSize!.Value, elementsCount);

            var paginationResponse = await CreatePaginationInternByCampaignResponse(paginationRequest, campaignId, pageCount);

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

        private async Task<PaginationResponse<InternSummaryResponse>> CreatePaginationInternByCampaignResponse(PaginationRequest paginationRequest, Guid campaignId, int pageCount)
        {
            int skip = CalculateSkipCount(paginationRequest!.PageNum!.Value, paginationRequest!.PageSize!.Value);

            var internsByCampaignSummaryResponse = await context
                .InternCampaigns
                .AsNoTracking()
                .Where(internCampaign =>
                    internCampaign.Person.PersonRoles.Any(pr => pr.RoleId == RoleId.Intern) &&
                    internCampaign.CampaignId == campaignId)
                .Select(ic => ic.Person.ToInternSummaryResponse())
                .Skip(skip)
                .Take(paginationRequest.PageSize.Value)
                .ToListAsync();

            var paginationResponse = new PaginationResponse<InternSummaryResponse>(
                internsByCampaignSummaryResponse,
                paginationRequest.PageNum.Value,
                pageCount);
            return paginationResponse;
        }

        private async Task<PaginationResponse<InternListingResponse>> CreatePaginationInternListingResponse(PaginationRequest paginationRequest, int elementsCount)
        {
            var pageCount = CalculatePageCount(paginationRequest!.PageSize!.Value, elementsCount);

            var skip = CalculateSkipCount(paginationRequest!.PageNum!.Value, paginationRequest!.PageSize!.Value);

            var interns = await context
                .Persons
                .Where(HasInternRole())
                .AsNoTracking()
                .Include(i => i.InternCampaigns)
                .Skip(skip)
                .Take(paginationRequest.PageSize.Value)
                .ToListAsync();

            var internListingResponses = interns
                .Select(i => i.ToInternListingResponse())
                .ToList();

            var paginationResponse = new PaginationResponse<InternListingResponse>(
                internListingResponses,
                paginationRequest.PageNum.Value,
                pageCount);

            return paginationResponse;
        }

        private static string CreateDisplayName(string firstName, string lastName)
        {
            return firstName + " " + lastName;
        }

        private static void UpdateDisplayName(UpdateInternRequest updateInternRequest, Person person)
        {

            if (person.WorkEmail == string.Empty)
            {
                person.DisplayName = CreateDisplayName(updateInternRequest.FirstName, updateInternRequest.LastName);
            }
        }

        private static Expression<Func<Person, bool>> HasInternRole()
        {
            return p => p.PersonRoles.Any(pr => pr.RoleId == RoleId.Intern);
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
