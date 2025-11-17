using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Domain.Exceptions;

namespace Neuraltech.SharedKernel.Application.UseCases.Base
{

    public abstract class BaseUseCase<TRequest>(ILogger logger) : BaseUseCase<TRequest, Unit>(logger)
    {

    }

    public abstract class BaseUseCase<TRequest, TResponse>(
        ILogger logger
    ) 
    {
        protected ILogger _logger = logger;
        public async ValueTask<UseCaseResponse<TResponse>> Execute(TRequest request)
        {
            _logger.LogInformation("Executing {0}", GetType().Namespace);
            _logger.LogDebug("Request: {0}", request);
            try
            { 
                var result = await ExecuteLogic(request);
                _logger.LogInformation("{0} executed succesfully", GetType().FullName);
                return result;
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex, "Domain error executing {0}", GetType().FullName);
                throw;
            }
            catch (Exception ex) { 
                _logger.LogError(ex, "Error executing {0}", GetType().FullName);
                throw;
            }
        }

        protected abstract ValueTask<UseCaseResponse<TResponse>> ExecuteLogic(TRequest request);

    }
}
