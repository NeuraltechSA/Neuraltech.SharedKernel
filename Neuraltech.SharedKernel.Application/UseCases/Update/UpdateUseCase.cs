using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Application.UseCases.Base;
using Neuraltech.SharedKernel.Domain.Base;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Domain.Exceptions;
using Neuraltech.SharedKernel.Domain.Services;

namespace Neuraltech.SharedKernel.Application.UseCases.Update
{
    public abstract class UpdateUseCase<TRequest, TEntity>(
        ILogger logger,
        IFindByIdRepository<TEntity> findByIdRepository,
        IUpdateRepository<TEntity> repository,
        IUnitOfWork unitOfWork
    ) : BaseUseCase<TRequest>(logger)
        where TRequest : UpdateDTO
        where TEntity : AggregateRoot
    {
        private readonly IUpdateRepository<TEntity> _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IFindByIdRepository<TEntity> _findByIdRepository = findByIdRepository;

        protected abstract TEntity Combine(TEntity entityToUpdate, TRequest request);

        protected override async ValueTask<UseCaseResponse<Unit>> ExecuteLogic(TRequest request)
        {
            var entityToUpdate = await _findByIdRepository.Find(request.Id);
            Ensure.NotNull(
                entityToUpdate,
                () => EntityToUpdateNotFoundException.CreateFromId(request.Id, typeof(TEntity))
            );

            _logger.LogInformation(
                $"Updating entity of type {typeof(TEntity).Name} with id {request.Id}"
            );
            var updatedEntity = Combine(entityToUpdate!, request);
            await _repository.Update(updatedEntity);

            await _unitOfWork.PublishEvents(updatedEntity.PullDomainEvents());
            await _unitOfWork.SaveChangesAndFlushEvents();

            _logger.LogInformation(
                $"Entity of type {typeof(TEntity).Name} with id {request.Id} updated successfully"
            );

            return UseCaseResponse.Empty();
        }
    }
}
