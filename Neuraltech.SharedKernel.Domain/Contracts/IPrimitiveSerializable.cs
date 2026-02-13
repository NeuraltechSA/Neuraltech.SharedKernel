namespace Neuraltech.SharedKernel.Domain.Contracts
{
    public interface IPrimitiveSerializable<TEntity, TPrimitives>
    {
        TPrimitives ToPrimitives();
        static abstract TEntity FromPrimitives(TPrimitives primitives);
    }
}
