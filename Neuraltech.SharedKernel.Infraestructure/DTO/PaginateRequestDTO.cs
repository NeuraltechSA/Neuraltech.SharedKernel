namespace Neuraltech.SharedKernel.Infraestructure.DTO
{
    public record PaginateRequestDTO
    {
        public long Page { get; init; } = 1;
        public long Limit { get; init; } = 20;
    }
}
