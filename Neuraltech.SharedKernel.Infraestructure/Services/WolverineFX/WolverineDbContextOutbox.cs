using Microsoft.EntityFrameworkCore;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using Wolverine.EntityFrameworkCore;
using Wolverine.Runtime;

namespace Neuraltech.SharedKernel.Infraestructure.Services.WolverineFX
{
    public class WolverineDbContextOutbox<T> : DbContextOutbox<T>, IUnitOfWork
        where T : DbContext
    {
        public WolverineDbContextOutbox(
            IWolverineRuntime runtime,
            T dbContext, 
            IEnumerable<IDomainEventScraper> scrapers
        ) : base(runtime, dbContext, scrapers)
        {
        }

        public async Task PublishEvents<TEvent>(List<TEvent> @event) where TEvent : BaseEvent
        {
            foreach (var item in @event)
            {
                await PublishAsync(item);
            }
        }

        public async Task SaveChangesAndFlushEvents(CancellationToken cancellationToken = default)
        {            
            await SaveChangesAndFlushMessagesAsync(cancellationToken);
        }
    }
}
