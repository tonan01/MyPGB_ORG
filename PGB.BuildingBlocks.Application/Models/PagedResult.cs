namespace PGB.BuildingBlocks.Application.Models
{
    public class PagedResult<T>
    {
        #region Properties
        /// <summary>
        /// Items in current page
        /// </summary>
        public IReadOnlyList<T> Items { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages { get; }
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages; 
        #endregion

        #region Constructor
        public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        public static PagedResult<T> Empty() => new(Array.Empty<T>(), 0, 1, 10); 
        #endregion
    }
}