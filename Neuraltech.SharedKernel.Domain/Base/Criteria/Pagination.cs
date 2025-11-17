using Neuraltech.SharedKernel.Domain.Services;

namespace Neuraltech.SharedKernel.Domain.Base.Criteria;

public sealed record Pagination
{
    public long Size { get; init; }
    public long Page { get; init; }

    public Pagination(long Size, long Page)
    {
        //EnsureSizeIsGreaterThanZero(Size);
        //EnsurePageIsGreaterThanZero(Page);
        //Ensure.(Size, 1, 100);
        //TODO: Validate

        this.Size = Size;
        this.Page = Page;
    }
}