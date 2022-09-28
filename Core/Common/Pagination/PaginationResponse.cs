namespace Core.Common.Pagination
{
    public record PaginationResponse<T>(IEnumerable<T> Content, int PageNum, int TotalPages);
}
