using System;
using System.Globalization;

namespace Blaven.BlogSources.Blogger
{
    public static class DateTimeExtensions
    {
        public static string ToRfc3339String(this DateTime dateTime)
        {
            string rfc3339 = dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
            return rfc3339;
        }
    }
}