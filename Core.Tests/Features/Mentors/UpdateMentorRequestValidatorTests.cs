using Core.Features.Mentors.RequestModels;
using Core.Features.Mentors.Support;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using Xunit;

namespace Core.Tests.Features.Mentors
{
    public class UpdateMentorRequestValidatorTests
    {
        UpdateMentorRequestValidator validator = new();

        private Guid id = Guid.NewGuid();
        
        private List<Guid> specialityIds = new List<Guid>() { Guid.NewGuid() };

        [Fact]
        public void Validator_WhenIdIsNotEmpty_ShouldNotHaveError()
        {
            var request = new UpdateMentorRequest(id, specialityIds);

            validator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(m => m.Id);
        }

        [Fact]
        public void Validator_WhenSpecialityIdsAreNotEmpty_ShouldNotHaveError()
        {
            var request = new UpdateMentorRequest(id, specialityIds);

            validator
                .TestValidate(request)
                .ShouldNotHaveValidationErrorFor(m => m.SpecialityIds);
        }

        [Fact]
        public void Validator_WhenIdIsEmpty_ShouldHaveError()
        {
            var request = new UpdateMentorRequest(Guid.Empty, specialityIds);

            validator
                .TestValidate(request)
                .ShouldHaveValidationErrorFor(m => m.Id);
        }

        [Fact]
        public void Validator_WhenSpecialityIdsAreEmpty_ShouldHaveError()
        {
            var emptySpecialityIds = new List<Guid>();

            var request = new UpdateMentorRequest(id, emptySpecialityIds);

            validator
                .TestValidate(request)
                .ShouldHaveValidationErrorFor(m => m.SpecialityIds);
        }
    }
}
