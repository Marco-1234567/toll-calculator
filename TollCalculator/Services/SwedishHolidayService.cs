using System;
using System.Collections.Generic;
using System.Text;

namespace TollCalculator.Services
{
    public class SwedishHolidayService
    {
        public bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday
                || date.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}
