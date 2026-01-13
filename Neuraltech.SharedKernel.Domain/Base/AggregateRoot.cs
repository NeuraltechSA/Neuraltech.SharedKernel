using Neuraltech.SharedKernel.Domain.ValueObjects;

namespace Neuraltech.SharedKernel.Domain.Base;

public abstract class AggregateRoot : AggregateRoot<UuidValueObject>
{
    protected AggregateRoot(UuidValueObject id) : base(id)
    {
    }
}

public abstract class AggregateRoot<TId> : Entity
{
    public TId Id { get; init; }

    private readonly List<BaseEvent> _domainEvents = new();
    
    protected AggregateRoot(TId id)
    {
        Id = id;
    }

    protected void RecordDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public List<BaseEvent> PullDomainEvents()
    {
        var events = new List<BaseEvent>(_domainEvents);
        _domainEvents.Clear();
        return events;
    }
}