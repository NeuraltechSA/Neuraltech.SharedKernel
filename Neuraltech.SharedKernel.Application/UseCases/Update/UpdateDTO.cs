namespace Neuraltech.SharedKernel.Application.UseCases.Update
{
    public abstract record UpdateDTO
    {
        public required Guid Id { get; init; }
    }
}
