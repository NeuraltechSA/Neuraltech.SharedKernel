using System.Text.Json;
using StackExchange.Redis;
using Neuraltech.SharedKernel.Domain.Base;
using System.Collections.Generic;
using System.Linq;
 
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Domain.Services;

namespace Neuraltech.SharedKernel.Infraestructure.Persistence.Redis;

/// <summary>
/// Base Redis repository implementing basic CRUD contracts (Create, FindById, Update, Delete).
/// Uses JSON serialization. Derived classes must provide KeyPrefix and entity Id extraction.
/// </summary>
public abstract class RedisRepository<TEntity, TEntityDTO> :
    ICreateRepository<TEntity>,
    IFindByIdRepository<TEntity>,
    IUpdateRepository<TEntity>,
    IDeleteRepository<TEntity>
    where TEntity : Entity
{
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly JsonSerializerOptions _jsonOptions;

    protected RedisRepository(
        IConnectionMultiplexer multiplexer, 
        JsonSerializerOptions? jsonOptions = null
    )
    {
        _multiplexer = multiplexer;
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    protected abstract string KeyPrefix { get; }
    protected abstract Guid GetEntityId(TEntity entity);
    protected virtual string GetEntityKey(TEntity entity) => GetKey(GetEntityId(entity));
    protected virtual string GetKey(Guid id) => $"{KeyPrefix}:{id}";

    protected IDatabase Db => _multiplexer.GetDatabase();

    protected virtual string Serialize(TEntityDTO entity) => JsonSerializer.Serialize(entity, _jsonOptions);
    protected virtual TEntityDTO? Deserialize(string? json) => JsonSerializer.Deserialize<TEntityDTO>(json!, _jsonOptions);

    protected abstract TEntity MapToEntity(TEntityDTO dto);
    protected abstract TEntityDTO MapToDTO(TEntity entity);

    public virtual async ValueTask Create(TEntity entity)
    {
        var key = GetEntityKey(entity);
        var json = Serialize(MapToDTO(entity));
        // Only set if not exists (natural create semantic). Caller should handle duplication logic.
        await Db.StringSetAsync(key, json, when: When.NotExists);
    }

    public virtual async ValueTask Update(TEntity entity)
    {
        var key = GetEntityKey(entity);
        var json = Serialize(MapToDTO(entity));
        await Db.StringSetAsync(key, json, when: When.Exists);
    }

    public virtual async ValueTask<TEntity?> Find(Guid id)
    {
        var key = GetKey(id);
        var json = await Db.StringGetAsync(key);
        var dto = Deserialize(json);
        Ensure.NotNull(dto);
        return MapToEntity(dto!);
    }

    public virtual async ValueTask Delete(TEntity entity)
    {
        var key = GetEntityKey(entity);
        await Db.KeyDeleteAsync(key);
    }

}
