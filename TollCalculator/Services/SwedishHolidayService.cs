namespace TollCalculator.Services
{
    /// <summary>
    /// Service for determining Swedish public holidays and weekends.
    /// Contains no toll specific logic — purely a calendar utility.
    /// </summary>
    public class SwedishHolidayService
    {
        /// <summary>
        /// Determines whether the given date is a Swedish public holiday.
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns>True if the date is a public holiday</returns>
        public bool IsPublicHoliday(DateTime date)
        {
            var holidays = GetHolidays(date.Year);
            return holidays.Contains(date.Date);
        }

        /// <summary>
        /// Determines whether the given date falls on a weekend.
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns>True if the date is a Saturday or Sunday</returns>
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

        public DateTime GetAllSaints(int year)
        {
            // First Saturday on or after October 31
            var date = new DateTime(year, 10, 31);
            while (date.DayOfWeek != DayOfWeek.Saturday)
                date = date.AddDays(1);
            return date;
        }

        public DateTime GetEaster(int year)
        {
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int month = (h + l - 7 * m + 114) / 31;
            int day = ((h + l - 7 * m + 114) % 31) + 1;
            return new DateTime(year, month, day);
        }

        private HashSet<DateTime> GetHolidays(int year)
        {
            var easter = GetEaster(year);

            return new HashSet<DateTime>
            {
                new DateTime(year, 1, 1),           // Nyårsdagen
                new DateTime(year, 1, 6),           // Trettondedag jul
                new DateTime(year, 5, 1),           // Första maj
                new DateTime(year, 6, 6),           // Nationaldagen
                new DateTime(year, 12, 25),         // Juldagen
                new DateTime(year, 12, 26),         // Annandag jul
                new DateTime(year, 12, 31),         // Nyårsafton
                GetMidsummerEve(year),              // Midsommarafton
                GetMidsummerEve(year).AddDays(1),   // Midsommardagen
                GetAllSaints(year),                 // Alla helgons dag
                easter.AddDays(-2),                 // Långfredag
                easter,                             // Påskdagen
                easter.AddDays(1),                  // Annandag påsk
                easter.AddDays(39),                 // Kristi himmelsfärd
                easter.AddDays(49),                 // Pingstdagen
            };
        }
    }
}
