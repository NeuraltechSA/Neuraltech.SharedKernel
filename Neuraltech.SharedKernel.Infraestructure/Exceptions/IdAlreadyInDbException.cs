

namespace Neuraltech.SharedKernel.Infraestructure.Exceptions
{
    public class IdAlreadyInDbException : Exception
    {
        public string DuplicatedId { get; init; }

        private IdAlreadyInDbException(string message, string id) : base(message)
        {
            DuplicatedId = id;
        }

        public static IdAlreadyInDbException Create(string id)
        {
            return new IdAlreadyInDbException(
                $"An entity with the id '{id}' already exists.",
                id
            );
        }
    }
}
