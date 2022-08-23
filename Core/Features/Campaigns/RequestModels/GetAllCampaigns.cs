using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Features.Campaigns.RequestModels
{
    public class GetAllCampaigns
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool OrderByIsActive { get; set; }
        public bool OrderAlsoByEndDate { get; set; }

    }
}
