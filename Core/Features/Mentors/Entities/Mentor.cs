using Core.Features.Campaigns.Entities;
using Core.Features.Mentors.Support;
using Core.Features.Specialties.Entities;
using System.ComponentModel.DataAnnotations;

namespace Core.Features.Mentors.Entities
{
    public class Mentor
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public ICollection<Speciality> Specialities { get; set; }

        public ICollection<Campaign> Campaigns { get; set; }

        public Mentor()
        {
        }
    }
}
