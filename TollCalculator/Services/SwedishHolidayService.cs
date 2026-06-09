using System;
using System.Collections.Generic;
using System.Text;

namespace TollCalculator.Services
{
    public class SwedishHolidayService
    {
        public bool IsPublicHoliday(DateTime date)
        {
            var fixedHolidays = GetFixedHolidays(date.Year);
            return fixedHolidays.Contains(date.Date);
        }

        private HashSet<DateTime> GetFixedHolidays(int year)
        {
            return new HashSet<DateTime>
    {
        new DateTime(year, 1, 1),   // Nyårsdagen
        new DateTime(year, 1, 6),   // Trettondedag jul
        new DateTime(year, 5, 1),   // Första maj
        new DateTime(year, 6, 6),   // Nationaldagen
        new DateTime(year, 12, 24), // Julafton
        new DateTime(year, 12, 25), // Juldagen
        new DateTime(year, 12, 26), // Annandag jul
        new DateTime(year, 12, 31), // Nyårsafton
        GetMidsummerEve(year),              // Midsommarafton
        GetMidsummerEve(year).AddDays(1),   // Midsommardagen
    };
        }

        public bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday
                || date.DayOfWeek == DayOfWeek.Sunday;
        }

        public DateTime GetMidsummerEve(int year)
        {
            // First Friday on or after June 19
            var date = new DateTime(year, 6, 19);
            while (date.DayOfWeek != DayOfWeek.Friday)
                date = date.AddDays(1);
            return date;
        }
    }
}
