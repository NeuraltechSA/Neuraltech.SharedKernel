namespace Neuraltech.SharedKernel.Domain.Exceptions
{
    public class IdAlreadyExistsException : DomainException
    {
        public IdAlreadyExistsException(string message) : base(message)
        {
        }

        public static IdAlreadyExistsException CreateFromId(Guid id) => new IdAlreadyExistsException($"Id {id} already exists");
    }
}
