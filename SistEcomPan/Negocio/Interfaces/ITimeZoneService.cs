using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaces
{
    public interface ITimeZoneService
    {
        TimeZoneInfo GetTimeZone(HttpRequest request);
        DateTime? ConvertirFecha(string?busqueda,TimeZoneInfo userTimeZone);
    }
}
