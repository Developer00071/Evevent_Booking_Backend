using Application.DTOs;

namespace Application.Interfaces;

public interface ITicketServer
{
    Task<TicketDto> GetTicket(Guid code);
    Task CheckIn(Guid code);
}
