using System.Collections.Generic;

namespace Neuraltech.SharedKernel.Domain.ValueObjects;

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNext => PageNumber < TotalPages;
    public bool HasPrevious => PageNumber > 1;
}