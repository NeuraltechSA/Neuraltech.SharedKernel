namespace Neuraltech.SharedKernel.Application.UseCases.Base
{
    public class UseCaseResponse<T>
    {
        public T? Value { get; init; }

        public static UseCaseResponse<T> FromResult(T result)
        {
            return new UseCaseResponse<T> { Value = result };
        }
    }

    public class UseCaseResponse : UseCaseResponse<Unit>
    {
        public static UseCaseResponse<Unit> Empty()
        {
            return new UseCaseResponse<Unit> { Value = null };
        }
    }

}