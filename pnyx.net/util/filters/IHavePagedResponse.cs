namespace pnyx.net.util.filters;

public interface IHavePagedResponse<TType> where TType : class
{
    public PagedResponse<TType> pagedResponse { get; set; }
}