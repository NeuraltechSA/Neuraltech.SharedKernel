using Neuraltech.SharedKernel.Domain.Base.Criteria.Ordering;

namespace Neuraltech.SharedKernel.Domain.Base.Criteria
{
    public interface IOrderable<T>
    {
        public T AddOrder(string propertyName, OrderTypes orderType);
        public List<Order> GetOrders();

        public bool HasOrder();
    }
}
