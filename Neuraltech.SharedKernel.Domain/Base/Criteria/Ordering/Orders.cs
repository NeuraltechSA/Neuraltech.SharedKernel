using System.Collections.Generic;
using System.Linq;

namespace Neuraltech.SharedKernel.Domain.Base.Criteria.Ordering;

public sealed record Orders(List<Order> Value)
{
    public override string ToString()
    {
        if (Value == null || !Value.Any())
            return "";
        return string.Join(",", Value.Select(o => o.ToString()));
    }
}
