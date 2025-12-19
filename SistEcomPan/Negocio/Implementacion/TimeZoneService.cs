using Microsoft.AspNetCore.Http;
using Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Implementacion
{
    public class TimeZoneService : ITimeZoneService
    {
        public DateTime? ConvertirFecha(string? busqueda, TimeZoneInfo userTimeZone)
        {
            if (!DateTime.TryParse(busqueda, out DateTime fechaLocal))
                return null;
            fechaLocal = DateTime.SpecifyKind(fechaLocal, DateTimeKind.Unspecified);

            return TimeZoneInfo.ConvertTimeToUtc(fechaLocal, userTimeZone);
        }

        public TimeZoneInfo GetTimeZone(HttpRequest request)
        {
            var timeZoneId = request.Headers["X-TimeZone"].FirstOrDefault();

            try
			{
                return !string.IsNullOrWhiteSpace(timeZoneId)
                    ? TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)
                    : TimeZoneInfo.Utc;

			}
			catch 
			{
                return TimeZoneInfo.Utc;
			}
        }
    }
}
