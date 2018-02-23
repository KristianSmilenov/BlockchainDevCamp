using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BlockchainCore.Utils
{
    public class DateTimeUtils
    {
        public static string GetISO8601DateFormat(DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
        }
    }
}
