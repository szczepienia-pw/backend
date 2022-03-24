using backend.Helpers;

namespace backend.Dto.Responses
{
    public class PaginatedResponse<U, T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int CurrentRecords { get; set; }
        public int TotalRecords { get; set; }
        public T Data { get; set; }

        public PaginatedResponse(PaginatedList<U> paginatedList, T data)
        {
            this.CurrentPage = paginatedList.CurrentPage;
            this.TotalPages = paginatedList.TotalPages;
            this.CurrentRecords = paginatedList.CurrentRecords;
            this.TotalRecords = paginatedList.TotalRecords;
            this.Data = data;
        }
    }
}
