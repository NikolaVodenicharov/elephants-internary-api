using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Features.Campaigns.RequestModels
{
    public class UpdateCampaign : CreateCampaign
    {
        public UpdateCampaign(Guid Id, string Name, DateTime StartDate, DateTime EndDate, bool IsActive)
            : base(Name, StartDate, EndDate, IsActive)
        {
            this.Id = Id;
        }

        public Guid Id { get; set; }
    }
}
