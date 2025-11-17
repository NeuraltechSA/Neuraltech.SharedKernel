

namespace Neuraltech.SharedKernel.Infraestructure.DTO
{
    public record PaginateResponseDTO<T>
    {
        public required long Count { get; init; }
        public required long Page { get; init; }
        public required long PageSize { get; init; }
        public required IEnumerable<T> Items { get; init; }
    }
}
