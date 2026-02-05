//using MassTransit;
using Microsoft.EntityFrameworkCore;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Services;
using Wolverine.EntityFrameworkCore;


namespace Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Contexts
{
    public abstract class 
        BaseDbContext<TContext> : DbContext, IUnitOfWork
        where TContext : DbContext
    {
        public BaseDbContext(DbContextOptions<TContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.MapWolverineEnvelopeStorage();

            /*
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();*/
        }

        public override int SaveChanges()
        {
            EntityTimestampUpdater.UpdateTimestamps(this);
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            EntityTimestampUpdater.UpdateTimestamps(this);
            return base.SaveChangesAsync(cancellationToken);
        }
         
    }
}
