using pnyx.net.api;
using pnyx.net.impl.columns;

namespace pnyx.net.fluent;

public class WithColumns : IRowFilterModifier, IRowTransformerModifer
{
    public ColumnIndex[] indexes { get; set; }

    public WithColumns(ColumnIndex[] indexes)
    {
        this.indexes = indexes;
    }

    public IRowFilter modifyRowFilter(IRowFilter rowFilter)
    {
        return new RowFilterWithColumns(indexes, rowFilter);
    }

    public IRowTransformer modifyRowTransformer(IRowTransformer rowTransformer)
    {
        return new RowTransformerWithColumns(indexes, rowTransformer);
    }
}