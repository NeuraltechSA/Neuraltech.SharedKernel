using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Models;


namespace Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Services;

public sealed class EntityTimestampUpdater{
    public static void UpdateTimestamps(DbContext context){

        var entries = GetEntries(context);


        foreach (var entry in entries)
        {
            var entity = (BaseEFCoreModel)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
                // Prevenir que se modifique CreatedAt
                entry.Property(nameof(BaseEFCoreModel.CreatedAt)).IsModified = false;
            }
        }
    }

    private static List<EntityEntry> GetEntries(DbContext context) {
        return context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEFCoreModel)
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .ToList();

    }


}