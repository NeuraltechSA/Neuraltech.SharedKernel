//using MassTransit;
using Microsoft.EntityFrameworkCore;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Infraestructure.Exceptions;
using Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Models;
using Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Services;
using System.Data.Common;
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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                EntityTimestampUpdater.UpdateTimestamps(this);
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch(DbUpdateException ex) 
            {
                TryThrowDuplicateKeyException(ex.InnerException);
                throw;
            }
        }

        protected void TryThrowDuplicateKeyException(Exception? ex)
        {
            if (ex is null || ex is not DbException dbEx) return;

            var entry = ChangeTracker.Entries().FirstOrDefault();
            if (entry is null) return;
            if (entry.Entity is not BaseEFCoreModel entity) return;

            List<string> sqlStates = ["23505", "1062", "2627"];
            if (sqlStates.Contains(dbEx.SqlState ?? ""))
            {
                throw IdAlreadyInDbException.Create(entity.Id.ToString());
            }
        }

    }
}