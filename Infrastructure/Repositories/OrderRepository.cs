using System.Data;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository(AppDbContext appDbContext) : IOrderRepository
{
    public IQueryable<Order> GetQuery() => appDbContext.Orders;
    public Task<List<Order>> GetOrders() => appDbContext.Orders.ToListAsync();
    public async Task<Order?> GetOrder(int id) => await appDbContext.Orders.FindAsync(id);
    public Task AddOrder(Order order) => appDbContext.Orders.AddAsync(order).AsTask();
    public void UpdateOrder(Order order) => appDbContext.Orders.Update(order);
    public void DeleteOrder(Order order) => appDbContext.Orders.Remove(order);
    public Task<int> SaveChangesAsync() => appDbContext.SaveChangesAsync();

    public async Task InTransactionAsync(Func<Task> action)
    {
        await using var transaction = await appDbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await action();
        await transaction.CommitAsync();
    }
}
