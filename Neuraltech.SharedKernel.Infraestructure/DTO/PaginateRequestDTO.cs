using Neuraltech.SharedKernel.Infraestructure.DTO;

namespace Neuraltech.SharedKernel.Application.UseCases.Paginate;

public record PaginateRequestDTO
{
    public virtual OptionalParam<long> Page { get; init; }
    public virtual OptionalParam<long> Limit { get; init; }
}
