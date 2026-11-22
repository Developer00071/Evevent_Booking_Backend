using Application.DTOs;

namespace Application.Interfaces;

public interface IOrderServer
{
    Task<OrderDto> AddOrder(CreateOrderDto dto);
    Task<OrderDto> GetOrder(int id);
    Task PayOrder(int id);
    Task CancelOrder(int id);
    Task ExpireOrders();
}
