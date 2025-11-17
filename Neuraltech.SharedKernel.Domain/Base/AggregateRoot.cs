
using Neuraltech.SharedKernel.Domain.ValueObjects;

namespace Neuraltech.SharedKernel.Domain.Base;

public abstract class AggregateRoot : Entity
{
    public UuidValueObject Id { get; init; }

    private readonly List<BaseEvent> _domainEvents = new();
    
    protected AggregateRoot(UuidValueObject id)
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