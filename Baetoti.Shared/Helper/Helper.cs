using Microsoft.AspNetCore.Http;
using System;

namespace Baetoti.Shared.Helper
{
    public static class Helper
    {
        public static double GetDistance(double Latitud1, double Longitude1, double Latitude2, double Longitude2)
        {
            double EarthRadius = 6371;

            double dLat = (Latitude2 - Latitud1) * Math.PI / 180;
            double dLon = (Longitude2 - Longitude1) * Math.PI / 180;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(Latitud1 * Math.PI / 180) * Math.Cos(Latitude2 * Math.PI / 180) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = EarthRadius * c;

            return distance;
        }

        public static string GetIPAddress(HttpContext httpContext)
        {
            return httpContext.Connection.RemoteIpAddress?.ToString();
        }

    }
}
