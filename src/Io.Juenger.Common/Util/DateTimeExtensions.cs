﻿// ReSharper disable once IdentifierTypo

using System;

// ReSharper disable once IdentifierTypo
namespace Io.Juenger.Common.Util
{
    /// <summary>
    ///     DateTime extensions methods
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Calculates number of business days, taking into account:
        ///         - weekends (Saturdays and Sundays)
        ///         - holidays in the middle of the week
        /// </summary>
        /// <remarks>I found this solution at <see aref="https://stackoverflow.com/a/1619375/6574264"/>.</remarks>
        /// <param name="firstDay">First day in the time interval</param>
        /// <param name="lastDay">Last day in the time interval</param>
        /// <param name="excludeDates">List of bank holidays excluding weekends</param>
        /// <returns>Number of business days during the 'span'</returns>
        public static int GetBusinessDaysUntil(
            this DateTime firstDay, 
            DateTime lastDay, 
            params DateTime[] excludeDates)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            var span = lastDay - firstDay;
            var businessDays = span.Days + 1;
            var fullWeekCount = businessDays / 7;
            
            // find out if there are weekends during the time exceeding the full weeks
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
            foreach (var bankHoliday in excludeDates)
            {
                var bh = bankHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return businessDays;
        }

        /// <summary>
        ///     Get total days until due date, considering:
        ///         - weekends (Saturdays and Sundays)
        ///         - holidays in the middle of the week
        /// </summary>
        /// <param name="startDate">Start date from where the due shall be calculated</param>
        /// <param name="requiredTotalWorkDays">Required total work days</param>
        /// <param name="excludeDates">Days to exclude</param>
        /// <returns>Number of total days till due date</returns>
        public static double GetTotalDaysUntilDueDate(
            this DateTime startDate, 
            float requiredTotalWorkDays, 
            params DateTime[] excludeDates)
        {
            var excludedDays = startDate.GetExcludedDays(requiredTotalWorkDays, excludeDates);
            var days = requiredTotalWorkDays + excludedDays;
            return days;
        }

        /// <summary>
        ///     Gets the due date from the passed required total work time, taking into account:
        ///         - weekends (Saturdays and Sundays)
        ///         - holidays in the middle of the week
        /// </summary>
        /// <remarks>
        ///     NOTE that the passed required total work time is pure time necessary to complete the project.
        /// </remarks>
        /// <param name="startDate">Start date from where the due shall be calculated</param>
        /// <param name="requiredTotalWorkTime">Required total work time</param>
        /// <param name="dailyWorkHours">Daily working hours</param>
        /// <param name="excludeDates">Days to exclude</param>
        /// <returns>Returns the due date</returns>
        public static DateTime GetBusinessDueDate(
            this DateTime startDate, 
            TimeSpan requiredTotalWorkTime, 
            float dailyWorkHours = 8,
            params DateTime[] excludeDates)
        {
            var requiredTotalWorkDays = requiredTotalWorkTime.TotalHours / dailyWorkHours;
            var dueDate = startDate.AddDays(requiredTotalWorkDays);
            var excludedDays = startDate.GetExcludedDays(requiredTotalWorkDays, excludeDates);
            dueDate = dueDate.AddDays(excludedDays);
            return dueDate;
        }

        /// <summary>
        ///      Gets the due date from the passed required total work time, taking into account:
        ///         - weekends (Saturdays and Sundays)
        ///         - holidays in the middle of the week    
        /// </summary>
        /// <param name="startDate">Start date from where the due shall be calculated</param>
        /// <param name="requiredTotalWorkDays">Required total work days</param>
        /// <param name="dailyWorkHours">Daily working hours</param>
        /// <param name="excludeDates">Days to exclude</param>
        /// <returns>Returns the due date</returns>
        public static DateTime GetBusinessDueDate(
            this DateTime startDate, 
            float requiredTotalWorkDays, 
            float dailyWorkHours = 8,
            params DateTime[] excludeDates)
        {
            var requiredTotalWorkTime = TimeSpan.FromHours(requiredTotalWorkDays * dailyWorkHours);
            return GetBusinessDueDate(startDate, requiredTotalWorkTime, dailyWorkHours, excludeDates);
        }
        
        private static double GetExcludedDays(
            this DateTime startDate, 
            double requiredTotalWorkDays, 
            params DateTime[] excludeDates)
        {
            var dueDate = startDate.AddDays(requiredTotalWorkDays);
            var businessDays = startDate.GetBusinessDaysUntil(dueDate, excludeDates);
            var daysToAdd = requiredTotalWorkDays - businessDays;
            return daysToAdd;
        }
    }
}