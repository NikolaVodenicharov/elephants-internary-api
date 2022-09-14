namespace Core.Common.Pagination
{
    public record PaginationResponse<T>(IEnumerable<T> Content, int CurrentPage, int TotalPages);
}
