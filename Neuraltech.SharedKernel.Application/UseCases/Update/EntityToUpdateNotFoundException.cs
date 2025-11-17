namespace Neuraltech.SharedKernel.Application.UseCases.Update
{
    public class EntityToUpdateNotFoundException : Exception
    {
        public EntityToUpdateNotFoundException(string message) :base(message)
        { }
        public static EntityToUpdateNotFoundException CreateFromId(Guid id) => new EntityToUpdateNotFoundException($"Entity with id {id} doesn't exist");
    }
}
