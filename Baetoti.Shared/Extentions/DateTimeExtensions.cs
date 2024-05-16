using System;

namespace Baetoti.Shared.Extentions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns TimeZone adjusted time for a given from a Utc or local time.
        /// Date is first converted to UTC then adjusted.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public static DateTime ToTimeZoneTime(this DateTime time, string timeZoneId = "Pakistan Standard Time")
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return time.ToTimeZoneTime(tzi);
        }

        /// <summary>
        /// Returns TimeZone adjusted time for a given from a Utc or local time.
        /// Date is first converted to UTC then adjusted.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public static DateTime ToTimeZoneTime(this DateTime time, TimeZoneInfo tzi)
        {
            if (time.Kind == DateTimeKind.Unspecified)
            {
                // Assume the DateTime is in the local time zone
                DateTime localTime = DateTime.SpecifyKind(time, DateTimeKind.Local);

                // Convert to the specified time zone, accounting for DST
                return TimeZoneInfo.ConvertTime(localTime, TimeZoneInfo.Local, tzi);
            }
            else
            {
                // Convert the DateTime to the specified time zone, accounting for DST
                return TimeZoneInfo.ConvertTime(time, tzi);
            }
        }

        /// <summary>
        /// Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).
        /// </summary>
        /// <param name="date">Date to convert</param>
        /// <returns>Returns Unix Time stamp</returns>
        public static long ToUnixEpochDate(this DateTime date)
        {
            return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        }

        /// <summary>
        /// Converts given date to Unix time stamp seconds
        /// </summary>
        /// <param name="date">Date to convert</param>
        /// <returns></returns>
        public static long ToUnixTimeSeconds(this DateTime date)
        {
            return new DateTimeOffset(date).ToUnixTimeSeconds();
        }

    }
}
