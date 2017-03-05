namespace TheatreProject.Helpers
{
    // Interface allows us to pass Paginator into 
    // views without having to worry about generic type.
    public interface IPaginator
    {
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
        bool IsPaginated { get; }
        int NextPageNumber { get; }
        int PreviousPageNumber { get; }
    }
}