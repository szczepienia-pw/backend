namespace backend.Helpers
{
    public class PaginatedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int CurrentRecords { get; private set; }
        public int TotalRecords { get; private set; }

        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            this.CurrentPage = pageNumber;
            this.TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            this.CurrentRecords = items.Count;
            this.TotalRecords = count;

            this.AddRange(items);
        }

        public static PaginatedList<T> Paginate(IQueryable<T> source, int pageNumber, int pageSize = 10)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
