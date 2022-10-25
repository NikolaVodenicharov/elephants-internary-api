using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Features.Admins.ResponseModels
{
    public record AdminListingResponse(
        Guid Id,
        string DisplayName,
        string WorkEmail,
        bool IsMentor);
}
