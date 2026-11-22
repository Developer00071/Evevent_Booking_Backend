using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderRepository
{
    IQueryable<Order> GetQuery();
    Task<List<Order>> GetOrders();
    Task<Order?> GetOrder(int id);
    Task AddOrder(Order order);
    void UpdateOrder(Order order);
    void DeleteOrder(Order order);
    Task<int> SaveChangesAsync();
    Task InTransactionAsync(Func<Task> action);
}
