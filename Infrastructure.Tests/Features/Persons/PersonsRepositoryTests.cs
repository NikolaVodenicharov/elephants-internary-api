using Core.Features.Persons.Entities;
using Core.Features.Persons.ResponseModels;
using Infrastructure.Features.Persons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests.Features.Persons
{
    public class PersonsRepositoryTests
    {
        private readonly InternaryContext context;
        private readonly PersonsRepository personsRepository;

        private readonly string personEmail = "john.doe@test.com";

        public PersonsRepositoryTests()
        {
            DbContextOptionsBuilder<InternaryContext>? dbOptions = new DbContextOptionsBuilder<InternaryContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString());

            context = new InternaryContext(dbOptions.Options);
            
            personsRepository = new PersonsRepository(context);

            AddRoles();
            AddPersonWithRoles();
        }

        [Fact]
        public async Task GetPersonRolesByEmailAsync_WhenPersonHasRoles_ShouldReturnCorrectObject()
        {
            // Act
            var personRolesResponse = await personsRepository.GetPersonRolesByEmailAsync(personEmail); 
            
            // Assert
            Assert.NotNull(personRolesResponse);
            Assert.NotEqual(Guid.Empty, personRolesResponse?.Id);
            Assert.Contains(RoleId.Mentor, personRolesResponse?.Roles);
            Assert.Contains(RoleId.Administrator, personRolesResponse?.Roles);
        }

        [Fact]
        public async Task GetPersonRolesByEmailAsync_WhenPersonNotFound_ShouldReturnCorrectObject()
        {
            // Arrange
            var newEmail = "jane.doe@test.com";
            
            // Act
            var personRolesResponse = await personsRepository.GetPersonRolesByEmailAsync(newEmail);

            // Assert
            Assert.Null(personRolesResponse);
        }

        private void AddRoles()
        {
            var roles = new List<Role>();
            roles.Add(new Role() { RoleId = RoleId.Administrator, Name = RoleId.Administrator.ToString() });
            roles.Add(new Role() { RoleId = RoleId.Intern, Name = RoleId.Intern.ToString() });
            roles.Add(new Role() { RoleId = RoleId.Mentor, Name = RoleId.Mentor.ToString() });

            context
                .Roles
                .AddRange(roles);

            context.SaveChanges();
        }

        private void AddPersonWithRoles()
        {
            var person = new Person()
            {
                DisplayName = "John Doe",
                WorkEmail = personEmail
            };

            var multipleRoles = new List<PersonRole>()
            {
                new PersonRole()
                {
                    RoleId = RoleId.Mentor,
                    Person = person,
                },
                new PersonRole()
                {
                    RoleId = RoleId.Administrator,
                    Person = person,
                },
            };

            context
                .PersonRoles
                .AddRange(multipleRoles);

            context.SaveChangesAsync();
        }
    }
}