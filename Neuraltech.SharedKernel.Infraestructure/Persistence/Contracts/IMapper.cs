namespace Neuraltech.SharedKernel.Infraestructure.Persistence.Contracts;

public interface IMapper<TEntity, TModel>
{
    TEntity MapToEntity(TModel model);
    TModel MapToModel(TEntity entity);
}