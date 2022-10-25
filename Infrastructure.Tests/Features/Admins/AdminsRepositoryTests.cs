using Core.Common.Pagination;
using Core.Features.Admins.RequestModels;
using Core.Features.Persons.Entities;
using Infrastructure.Features.Admins;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests.Features.Admins
{
    public class AdminsRepositoryTests
    {
        private readonly InternaryContext context;
        private readonly AdminsRepository adminsRepository;
        private readonly CreateAdminRepoRequest createAdminRequest;
        private readonly CreateAdminRepoRequest createAdditionalAdminRequest;

        public AdminsRepositoryTests()
        {
            DbContextOptionsBuilder<InternaryContext>? dbOptions = new DbContextOptionsBuilder<InternaryContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString());

            context = new InternaryContext(dbOptions.Options);
            
            adminsRepository = new AdminsRepository(context);

            createAdminRequest = new CreateAdminRepoRequest("John Doe", "John.Doe@test.com");
            createAdditionalAdminRequest = new CreateAdminRepoRequest("Jane Doe", "Jane.Doe@test.com");
        }

        [Fact]
        public async Task CreateAsync_AddSingleElement_ShouldAddToDatabase()
        {
            // Act
            var adminSummary = await adminsRepository.CreateAsync(createAdminRequest);

            // Assert
            var personRoles = await context
                .PersonRoles
                .Where(p => p.PersonId == adminSummary.Id)
                .FirstOrDefaultAsync();

            Assert.NotNull(adminSummary);
            Assert.NotEqual(Guid.Empty, adminSummary.Id);
            Assert.Equal(createAdminRequest.DisplayName, adminSummary.DisplayName);
            Assert.Equal(createAdminRequest.WorkEmail, adminSummary.WorkEmail);
            Assert.Equal(RoleId.Administrator, personRoles!.RoleId);
        }

        [Fact]
        public async Task GetByIdAsync_WhenFound_ShouldReturnCorrectObject()
        {
            // Arrange
            await adminsRepository.CreateAsync(createAdminRequest);

            var admin = await context.Persons.FirstOrDefaultAsync(p => p.WorkEmail == createAdminRequest.WorkEmail);

            // Act
            var adminSummary = await adminsRepository.GetByIdAsync(admin!.Id);

            // Assert
            Assert.NotNull(adminSummary);
            Assert.Equal(admin.Id, adminSummary!.Id);
            Assert.Equal(admin.DisplayName, adminSummary.DisplayName);
            Assert.Equal(admin.WorkEmail, adminSummary.WorkEmail);
        }
        
        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ShouldReturnNull()
        {
            // Arrange
            var adminId = Guid.NewGuid();

            // Act
            var adminSummary = await adminsRepository.GetByIdAsync(adminId);

            // Assert
            Assert.Null(adminSummary);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotEmpty_ShouldReturnCorrectCountElements()
        {
            // Arrange
            await adminsRepository.CreateAsync(createAdminRequest);
            await adminsRepository.CreateAsync(createAdditionalAdminRequest);

            var paginationRequest = new PaginationRequest(1, 5);

            // Act
            var paginationResponse = await adminsRepository.GetAllAsync(paginationRequest);

            // Assert
            Assert.Equal(2, paginationResponse.Count());
            Assert.False(paginationResponse.First().IsMentor);
        }
        
        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var paginationRequest = new PaginationRequest(1, 5);

            // Act
            var paginationResponse = await adminsRepository.GetAllAsync(paginationRequest);

            // Assert
            Assert.Empty(paginationResponse);
        }

        [Fact]
        public async Task GetCountAsync_WhenNotEmpty_ShouldReturnCorrectCount()
        {
            // Arrange
            await adminsRepository.CreateAsync(createAdminRequest);
            await adminsRepository.CreateAsync(createAdditionalAdminRequest);

            // Act
            var adminsCount = await adminsRepository.GetCountAsync();

            // Assert
            Assert.Equal(2, adminsCount);
        }
        
        [Fact]
        public async Task GetCountAsync_WhenEmpty_ShouldReturnZero()
        {
            // Act
            var count = await adminsRepository.GetCountAsync();
            
            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task ExistsByEmailAsync_WhenExists_ShouldReturnTrue()
        {
            // Arrange
            var adminEmail = createAdminRequest.WorkEmail;

            await adminsRepository.CreateAsync(createAdminRequest);

            // Act
            var existsByEmail = await adminsRepository.ExistsByEmailAsync(adminEmail);
            
            // Assert
            Assert.True(existsByEmail);
        }

        [Fact]
        public async Task ExistsByEmailAsync_WhenNotExists_ShouldReturnFalse()
        {
           // Arrange
            var adminEmail = "test.test@test.com";

            // Act
            var existsByEmail = await adminsRepository.ExistsByEmailAsync(adminEmail);
            
            // Assert
            Assert.False(existsByEmail);
        }
    }
}