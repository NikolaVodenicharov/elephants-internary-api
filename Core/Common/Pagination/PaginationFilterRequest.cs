namespace Core.Common.Pagination
{
    public class PaginationFilterRequest
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public int Count { get; set; }
    }
}
