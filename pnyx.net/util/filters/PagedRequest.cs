namespace pnyx.net.util.filters;

public class PagedRequest
{
    /// <summary>
    /// Page number to retrieve. Starts at 1. Required for a paged request.
    /// </summary>
    public int pageNumber { get; set; } = 1;
    
    /// <summary>
    /// Number of records per page. Required for a paged request.
    /// </summary>
    public int recordsPerPage { get; set; }
    
    /// <summary>
    /// Total number of records available for the given query.
    /// NOTE: Field is required to trigger pagination in responses
    /// </summary>
    public int totalRecords { get; set; }
    
    /// <summary>
    /// Total number of pages available for the given query. Calculated based on totalRecords and recordsPerPage.
    /// NOTE: Field is required to trigger pagination in responses
    /// </summary>
    public int totalPages { get; set; }

    public int recordsToSkip()
    {
        if (pageNumber <= 0)
            return 0;

        return (pageNumber - 1) * recordsPerPage;
    }

    public bool hasPrevious()
    {
        return pageNumber > 1;
    }

    public bool hasNext()
    {
        return pageNumber < totalPages;
    }

    public bool hasTotals()
    {
        return recordsPerPage > 0 && totalRecords > 0 && totalPages > 0 && pageNumber > 0;
    }
}