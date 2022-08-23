using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Features.Campaigns.ResponseModels
{
    public record CampaignSummary (Guid Id, string Name, DateTime StartDate, DateTime EndDate, bool IsActive);
}
