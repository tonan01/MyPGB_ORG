using PGB.BuildingBlocks.Application.Models;

namespace PGB.BuildingBlocks.Application.Queries
{
    public abstract class PagedQuery<TResponse> : BaseQuery<PagedResult<TResponse>>
    {
        #region Pagination Properties
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 10;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
        #endregion

        #region Validation & Normalization
        public virtual void ValidateAndNormalize()
        {
            if (Page < 1)
                Page = 1;

            if (PageSize < 1)
                PageSize = DefaultPageSize;
            else if (PageSize > MaxPageSize)
                PageSize = MaxPageSize;

            SearchTerm = SearchTerm?.Trim();
            SortBy = SortBy?.Trim();
        } 
        #endregion
    }
}