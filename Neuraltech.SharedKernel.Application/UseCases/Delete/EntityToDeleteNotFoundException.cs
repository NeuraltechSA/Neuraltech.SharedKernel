namespace Neuraltech.SharedKernel.Application.UseCases.Delete
{
    public class EntityToDeleteNotFoundException : Exception
    {
        public EntityToDeleteNotFoundException(string message) :base(message)
        { }
        public static EntityToDeleteNotFoundException CreateFromId(Guid id) => new EntityToDeleteNotFoundException($"Entity with id {id} doesn't exist");
    }
}
