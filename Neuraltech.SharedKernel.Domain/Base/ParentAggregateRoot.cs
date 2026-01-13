using Neuraltech.SharedKernel.Domain.ValueObjects;

namespace Neuraltech.SharedKernel.Domain.Base
{
    public class ParentAggregateRoot<TDiscriminator> : ParentAggregateRoot<TDiscriminator, UuidValueObject>
        where TDiscriminator : Enum
    {
        protected ParentAggregateRoot(UuidValueObject id, TDiscriminator type) : base(id, type)
        {
        }
    }
    public class ParentAggregateRoot<TDiscriminator,TId> : AggregateRoot<TId>
        where TDiscriminator : Enum
    {
        public TDiscriminator Type { get; init; }
        protected ParentAggregateRoot(TId id, TDiscriminator type) : base(id)
        {
            Type = type;
        }
    }
}
