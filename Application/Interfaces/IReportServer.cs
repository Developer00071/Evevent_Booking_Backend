using Application.DTOs;

namespace Application.Interfaces;

public interface IReportServer
{
    Task<List<TicketTypeSalesDto>> GetEventSales(int id);
    Task<List<TopEventDto>> GetTopEvents(int count);
}
