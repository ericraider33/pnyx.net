using System.Collections.Generic;

namespace pnyx.net.util.filters;

public class PagedResponse<TType> where TType : class
{
    public List<TType> records { get; set; }
    public int currentPage { get; set; }
    public int totalRecords { get; set; }
    public int totalPages { get; set; }
    public int recordsPerPage { get; set; }

    public bool hasNextPage => currentPage < totalPages;
    public bool hasPreviousPage => currentPage > 1;

    public PagedResponse(List<TType> records)
    {
        this.records = records;
    }

    public PagedResponse(List<TType> records, int currentPage, int totalRecords, int totalPages, int recordsPerPage)
    {
        this.records = records;
        this.currentPage = currentPage;
        this.totalRecords = totalRecords;
        this.totalPages = totalPages;
        this.recordsPerPage = recordsPerPage;
    }

    public static PagedResponse<TType> fromRequest(PagedRequest request, List<TType> records)
    {
        return new PagedResponse<TType>(
            records, 
            request.pageNumber, 
            request.totalRecords,
            request.totalPages,
            request.recordsPerPage
            );
    }

    public static PagedResponse<TType> firstPage(PagedRequest request, List<TType> records, int totalRecords)
    {
        PagedResponse<TType> pageInfo = new (records);
        pageInfo.currentPage = 1;
        pageInfo.totalRecords = totalRecords;
        pageInfo.totalPages = (pageInfo.totalRecords - 1) / request.recordsPerPage + 1;
        pageInfo.recordsPerPage = request.recordsPerPage;

        return pageInfo;
    }
}