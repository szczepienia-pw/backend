using backend.Helpers;

namespace backend.Dto.Responses
{
    public class PaginatedResponse<U, T>
    {
        public object Pagination { get; set; }
        public T Data { get; set; }

        public PaginatedResponse(PaginatedList<U> paginatedList, T data)
        {
            this.Pagination = new
            {
                CurrentPage = paginatedList.CurrentPage,
                TotalPages = paginatedList.TotalPages,
                CurrentRecords = paginatedList.CurrentRecords,
                TotalRecords = paginatedList.TotalRecords
            };

            this.Data = data;
        }
    }
}
