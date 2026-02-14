using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Services;

namespace Neuraltech.SharedKernel.Domain.Base.Criteria;

public abstract record Pagination
{
    private const long DefaultPageLimit = 1000;
    private const long DefaultSizeLimit = 100;

    public virtual long DefaultPage => 1;
    public virtual long DefaultSize => 20;

    protected virtual long MaxPageAllowed => DefaultPageLimit;
    protected virtual long MaxSizeAllowed => DefaultSizeLimit;

    public long Size { get; init; }
    public long Page { get; init; }

    public Pagination(Optional<long> Size, Optional<long> Page)
    {
        var page = Page.GetValueOrDefault(DefaultPage);
        var size = Size.GetValueOrDefault(DefaultSize);

        Ensure.InRange(page, 0, MaxPageAllowed);
        Ensure.InRange(size, 0, MaxSizeAllowed);

        this.Size = size;
        this.Page = page;
    }

    public override string ToString() => $"{Page},{Size}";
}
