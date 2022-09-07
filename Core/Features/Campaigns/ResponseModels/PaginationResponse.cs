﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Features.Campaigns.ResponseModels
{
    public record PaginationResponse<T>(IEnumerable<T> Content, int CurrentPage, int TotalPages);
}
