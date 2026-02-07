using System.Collections.Generic;
using System.Linq;

namespace Neuraltech.SharedKernel.Domain.Base.Criteria.Filtering;

public sealed record Filters(List<Filter> Value)
{
    public override string ToString()
    {
        if (Value == null || !Value.Any())
            return "";
        var ordered = Value.OrderBy(f => f.GetField()).Select(f => f.ToString());
        return string.Join(",", ordered);
    }
}
