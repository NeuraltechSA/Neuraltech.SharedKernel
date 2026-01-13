namespace Neuraltech.SharedKernel.Domain.Base.Criteria
{
    public interface IPaginable<T> where T : IPaginable<T>
    {
        T Paginate(long page, long pageSize);

        bool HasPagination();

        long? GetPageSize();

        long? GetPageNumber();

        long? GetPageNumberFromZero();
    }
}
