using EasyCaching.Core;
using Microsoft.EntityFrameworkCore;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Base.Criteria;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Services;

namespace Neuraltech.SharedKernel.Infraestructure.Persistence.EasyCaching.Services;


public abstract class EasyCachingRepository<TEntity, TCriteria, TKey>(
    IEasyCachingProvider cachingProvider,
    LinqCriteriaConverter linqCriteriaConverter
) :
    ICreateRepository<TEntity>,
    IUpdateRepository<TEntity>,
    IDeleteRepository<TEntity>,
    IPaginateRepository<TEntity, TCriteria>
    where TEntity : Entity
    where TCriteria : BaseCriteria<TCriteria>
{
    private readonly IEasyCachingProvider _cachingProvider = cachingProvider;
    private readonly LinqCriteriaConverter _linqCriteriaConverter = linqCriteriaConverter;
    protected virtual TimeSpan DefaultExpiration => TimeSpan.FromHours(1);
    protected abstract string KeyPrefix { get; }
    protected abstract TKey GetEntityKey(TEntity entity);
    private string GetKey(TEntity entity) => $"{KeyPrefix}:{GetEntityKey(entity)}";

    public virtual async ValueTask Create(TEntity entity)
    {
        var key = GetKey(entity);
        var exists = await _cachingProvider.ExistsAsync(key);
        if (exists)
        { 
            throw new InvalidOperationException($"Entity with key '{key}' already exists. Use Update instead.");
        }

        await _cachingProvider.SetAsync(key, entity, DefaultExpiration);
    }

    public virtual async ValueTask Update(TEntity entity)
    {
        var key = GetKey(entity);

        await _cachingProvider.SetAsync(key, entity, DefaultExpiration);
    }

    public virtual async ValueTask Delete(TEntity entity)
    {
        var key = GetKey(entity);
        await _cachingProvider.RemoveAsync(key);
    }
    protected async ValueTask<IEnumerable<TEntity>> GetAll()
    {
        var keys = await _cachingProvider.GetAllKeysByPrefixAsync(KeyPrefix);
        if (keys.Count() == 0) return [];
        var items =  _cachingProvider.GetAll<TEntity>(keys).Select(kv => kv.Value.Value);
        return items;
    }
    public async ValueTask<IEnumerable<TEntity>> Find(TCriteria criteria)
    {
        //TODO: CancellationToken
        var items = await GetAll();
        var filteredItems = _linqCriteriaConverter.Apply(criteria, items.AsQueryable());
        return filteredItems;
    }

    public async ValueTask<long> Count(TCriteria criteria)
    {
        var items = await GetAll();
        return await _linqCriteriaConverter.Apply(criteria, items.AsQueryable()).LongCountAsync();
    }
}
