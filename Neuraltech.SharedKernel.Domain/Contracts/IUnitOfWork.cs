
using Neuraltech.SharedKernel.Domain.Base;

namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface IUnitOfWork
    {
        Task PublishEvents<TEvent>(List<TEvent> @event) where TEvent : BaseEvent;
        Task SaveChangesAndFlushEvents(CancellationToken cancellationToken = default);
    }
}
