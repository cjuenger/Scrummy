// ReSharper disable once IdentifierTypo

using System;

// ReSharper disable once IdentifierTypo
namespace Scrummy.DataAccess.GitLab.Util
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Calculates number of business days, taking into account:
        ///  - weekends (Saturdays and Sundays)
        ///  - bank holidays in the middle of the week
        /// </summary>
        /// <remarks><see aref="https://stackoverflow.com/a/1619375/6574264"/></remarks>
        /// <param name="firstDay">First day in the time interval</param>
        /// <param name="lastDay">Last day in the time interval</param>
        /// <param name="bankHolidays">List of bank holidays excluding weekends</param>
        /// <returns>Number of business days during the 'span'</returns>
        public static int GetBusinessDaysUntil(this DateTime firstDay, DateTime lastDay, params DateTime[] bankHolidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            var span = lastDay - firstDay;
            var businessDays = span.Days + 1;
            var fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount*7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                var firstDayOfWeek = (int) firstDay.DayOfWeek;
                var lastDayOfWeek = (int) lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                switch (firstDayOfWeek)
                {
                    // Both Saturday and Sunday are in the remaining time interval
                    case <= 6 when lastDayOfWeek >= 7:
                        businessDays -= 2;
                        break;
                    case <= 6:
                    {
                        if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                            businessDays -= 1;
                        break;
                    }
                    // Only Sunday is in the remaining time interval
                    case <= 7 when lastDayOfWeek >= 7:
                        businessDays -= 1;
                        break;
                }
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            foreach (var bankHoliday in bankHolidays)
            {
                var bh = bankHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return businessDays;
        }
    }
}