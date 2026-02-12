namespace Neuraltech.SharedKernel.Domain.Exceptions
{
    public class EntityToDeleteNotFoundException : DomainException
    {
        public Type EntityType { get; init; }
        public Guid EntityId { get; init; }

        public EntityToDeleteNotFoundException(string message, Type entityType, Guid entityId) : base(message)
        {
            EntityType = entityType;
            EntityId = entityId;
        }

        public static EntityToDeleteNotFoundException 
            CreateFromId(Guid id, Type entityType) => 
                new($"Entity of type {entityType.Name} with id {id} doesn't exist", entityType, id);
    }
}
