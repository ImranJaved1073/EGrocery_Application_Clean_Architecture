namespace Domain
{
    public class PaginatedList
    {
        public int PageIndex { get; set; }
        public int TotalPages { get;private set; }

        public int PageSize { get; set; }

        public int TotalItems {  get; private set; }

        public int StartPage { get; set; }
        public int EndPage { get; set; }

        PaginatedList()
        {
        }

        public PaginatedList(int pageIndex, int totalPages, int pageSize, int totalItems)
        {
            PageIndex = pageIndex;
            TotalPages = totalPages;
            PageSize = pageSize;
            TotalItems = totalItems;

            StartPage = PageIndex - 5;
            EndPage = PageIndex + 4;

            if (StartPage <= 0)
            {
                EndPage -= (StartPage - 1);
                StartPage = 1;
            }

            if (EndPage > TotalPages)
            {
                EndPage = TotalPages;
                if (EndPage > 10)
                {
                    StartPage = EndPage - 9;
                }
            }
        }


        public bool HasPreviousPage => (PageIndex > 1);

        public bool HasNextPage => (PageIndex < TotalPages);

        public int FirstItemIndex => (PageIndex - 1) * PageSize + 1;

        public int LastItemIndex => Math.Min(PageIndex * PageSize, TotalItems);

        

    }
}
