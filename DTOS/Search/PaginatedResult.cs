namespace Minimart_Api.DTOS.Search
{
    public class PaginatedResult<T>  // <-- Add <T> here
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
    }
}