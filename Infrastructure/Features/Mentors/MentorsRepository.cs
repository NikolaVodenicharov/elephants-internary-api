using Core.Common.Pagination;
using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Support;
using Core.Features.Mentors.Interfaces;
using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.ResponseModels;
using Core.Features.Persons.Entities;
using Core.Features.Specialities.Support;
using Infrastructure.Features.Persons.Support;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Features.Mentors
{
    public class MentorsRepository : IMentorsRepository
    {
        private readonly InternaryContext context;

        public MentorsRepository(InternaryContext context)
        {
            this.context = context;
        }

        public async Task<MentorSummaryResponse> CreateAsync(CreateMentorRepoRequest createMentorRepoRequest)
        {
            var person = new Person()
            {
                DisplayName = createMentorRepoRequest.DisplayName,
                WorkEmail = createMentorRepoRequest.WorkEmail,
                Specialities = createMentorRepoRequest.Specialities
            };

            var personRole =
                new PersonRole()
                {
                    RoleId = RoleId.Mentor,
                    Person = person,
                };

            await context
                .PersonRoles
                .AddAsync(personRole);

            await context.SaveChangesAsync();

            return person.ToMentorSummaryResponse();
        }

        public async Task<bool> AddToCampaignAsync(AddMentorToCampaignRepoRequest addMentorToCampaignRepoRequest)
        {
            var person = await context
                .Persons
                .Where(p => p.Id == addMentorToCampaignRepoRequest.MentorId)
                .Where(HasMentorRole())
                .FirstOrDefaultAsync();

            if (person == null)
            {
                return false;
            }

            person.Campaigns = new List<Campaign>() { addMentorToCampaignRepoRequest.Campaign};

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<MentorDetailsResponse?> GetByIdAsync(Guid id)
        {
            var person = await context
                .Persons
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Where(HasMentorRole())
                .Select(p => p)
                .Include(p => p.Campaigns)
                .Include(p => p.Specialities)
                .FirstOrDefaultAsync();

            var mentorDetailsResponse = person?.ToMentorDetailsResponse();

            return mentorDetailsResponse;
        }

        public async Task<MentorDetailsResponse?> UpdateAsync(UpdateMentorRepoRequest updateMentorRepoRequest)
        {
            var person = await context
                .Persons
                .Where(p => p.Id == updateMentorRepoRequest.Id)
                .Where(HasMentorRole())
                .Include(p => p.Specialities)
                .Include(p => p.Campaigns)
                .FirstOrDefaultAsync();

            if (person == null)
            {
                return null;
            }

            person.Specialities = updateMentorRepoRequest.Specialities;

            await context.SaveChangesAsync();

            return person.ToMentorDetailsResponse();
        }

        public async Task<int> GetCountAsync()
        {
            var mentorCount = await context
                .Persons
                .Where(HasMentorRole())
                .CountAsync();

            return mentorCount;
        }

        public async Task<bool> IsEmailUsed(string email)
        {
            return await context
                .Persons
                .AnyAsync(u => u.WorkEmail.Equals(email));
        }

        public async Task<IEnumerable<MentorPaginationResponse>> GetAllAsync(PaginationRequest? filter = null, Guid? campaignId = null)
        {
            var skip = filter != null ? (filter!.PageNum!.Value - 1) * filter!.PageSize!.Value : 0;
            var take = filter != null ? filter!.PageSize!.Value : await GetCountAsync();

            var mentors = await context
                .Persons
                .AsNoTracking()
                .Where(HasMentorRole())
                .Where(m => (campaignId == null) || m.Campaigns.Any(c => c.Id == campaignId))
                .Include(m => m.Specialities)
                .Include(m => m.Campaigns)
                .OrderBy(m => m.Id)
                .Skip(skip)
                .Take(take)
                .Select(m => m.ToMentorPaginationResponse())
                .ToListAsync();

            return mentors;
        }

        public async Task<int> GetCountByCampaignIdAsync(Guid campaignId)
        {
            var count = await context
                .Persons
                .Where(HasMentorRole())
                .Where(HasCampaign(campaignId))
                .CountAsync();

            return count;
        }

        private static Expression<Func<Person, bool>> HasCampaign(Guid campaignId)
        {
            return person => person
                .Campaigns
                .Any(campaign => campaign.Id == campaignId);
        }

        private static Expression<Func<Person, bool>> HasMentorRole()
        {
            return person => person
                .PersonRoles
                .Any(personRole => personRole.RoleId == RoleId.Mentor);
        }
    }
}
