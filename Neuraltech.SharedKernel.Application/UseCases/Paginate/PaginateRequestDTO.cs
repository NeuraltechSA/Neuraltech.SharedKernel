namespace Neuraltech.SharedKernel.Application.UseCases.Paginate;

public record PaginateRequestDTO
{
    public long Page { get; init; } = 1;
    public long Limit { get; init; } = 20;
}
