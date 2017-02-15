namespace TheatreProject.Helpers
{
    public interface IPaginator
    {
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
        bool IsPaginated { get; }
        int NextPageNumber { get; }
        int PreviousPageNumber { get; }
    }
}