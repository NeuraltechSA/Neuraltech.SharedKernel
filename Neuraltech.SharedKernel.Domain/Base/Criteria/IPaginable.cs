namespace Neuraltech.SharedKernel.Domain.Base.Criteria
{
    public interface IPaginable<T> where T : IPaginable<T>
    {
        T Paginate(Optional<long> page, Optional<long> size);

        bool HasPagination();

        long? GetPageSize();

        long? GetPageNumber();

        long? GetPageNumberFromZero();
    }
}
