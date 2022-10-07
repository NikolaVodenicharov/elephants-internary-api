namespace Core.Features.Mentors.RequestModels
{
    public class UpdateMentorRequest
    {
        public Guid Id { get; set; }

        public IEnumerable<Guid> SpecialityIds { get; set; }

        public UpdateMentorRequest(Guid id, IEnumerable<Guid> specialityIds)
        {
            Id = id;
            SpecialityIds = specialityIds;
        }
    }
}
