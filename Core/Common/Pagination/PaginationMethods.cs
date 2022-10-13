using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Pagination
{
    public static class PaginationMethods
    {
        public static int CalculateTotalPages(int count, int pageSize)
        {
            var totalPages = count > 0 ?
                (count + pageSize - 1) / pageSize :
                PaginationConstants.DefaultPageCount;

            return totalPages;
        }
    }
}
