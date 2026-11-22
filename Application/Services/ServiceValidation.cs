using System.Net;
using Application;

namespace Application.Services;

public static class ServiceValidation
{
    public static void ValidateDates(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
        {
            throw new ApiException((int)HttpStatusCode.BadRequest, "EndDate must be later than StartDate.");
        }
    }
}
