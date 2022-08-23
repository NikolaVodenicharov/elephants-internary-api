using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Features.Campaigns.RequestModels
{
    public class CreateCampaign
    {
        public CreateCampaign(string Name, DateTime StartDate, DateTime EndDate, bool IsActive)
        {
            this.Name = Name;
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.IsActive = IsActive;
        }

        public string Name
        {
            get
            {
                return this.Name;
            }

            private set
            {
                this.Name = value;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return this.StartDate;
            }

            private set
            {
                this.StartDate = value;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return this.EndDate;
            }

            private set
            {
                this.EndDate = value;
            }
        }

        public bool IsActive { get; private set; }
    }
}
