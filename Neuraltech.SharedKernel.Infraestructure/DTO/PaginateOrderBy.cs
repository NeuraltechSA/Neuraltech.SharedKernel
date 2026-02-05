using Neuraltech.SharedKernel.Domain.Base.Criteria.Ordering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neuraltech.SharedKernel.Infraestructure.DTO
{
    public class PaginateOrderBy<TField>
        where TField : Enum
    {
        public required TField Field { get; set; }
        public OrderTypes Type { get; set; } = OrderTypes.DESC;
    }
}
