using Core.Features.Admins.RequestModels;
using Core.Features.Admins.Support;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using Xunit;

namespace Core.Tests.Features.Admins
{
    public class AddMentorRoleRequestValidatorTests
    {
        private readonly AddMentorRoleRequestValidator addMentorRoleValidator = new();
        private readonly Guid personId = Guid.NewGuid();
        private List<Guid> specialityIds = new List<Guid>()
        {
            Guid.NewGuid()
        };

        [Fact]
        public void Validator_WhenDataIsValid_ShouldNotHaveError()
        {
            var addMentorRoleRequest = new AddMentorRoleRequest(personId, specialityIds);

            addMentorRoleValidator
                .TestValidate(addMentorRoleRequest)
                .ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validator_WhenIdIsEmpty_ShouldHaveError()
        {
            var addMentorRoleRequest = new AddMentorRoleRequest(Guid.Empty, specialityIds);

            addMentorRoleValidator
                .TestValidate(addMentorRoleRequest)
                .ShouldHaveValidationErrorFor(f => f.Id);
        }

        [Fact]
        public void Validator_WhenSpecialityIdsAreEmpty_ShouldHaveError()
        {
            var addMentorRoleRequest = new AddMentorRoleRequest(personId, new List<Guid>());

            addMentorRoleValidator
                .TestValidate(addMentorRoleRequest)
                .ShouldHaveValidationErrorFor(f => f.SpecialityIds);
        }
    }
}