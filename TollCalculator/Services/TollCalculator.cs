using TollCalculator.Models;

namespace TollCalculator.Services
{
    public class TollCalculatorService
    {
        private const decimal DailyFeeCap = 60;
        private const decimal MinFee = 8;
        private const decimal MaxFee = 18;

        private readonly VehicleRegistry _vehicleRegistry;
        private readonly SwedishHolidayService _swedishHolidayService;

        public TollCalculatorService(VehicleRegistry reg, SwedishHolidayService holidays)
        {
            _vehicleRegistry = reg;
            _swedishHolidayService = holidays;
        }


        /// <summary>
        /// Calculate total toll fee for a list of toll entries.
        /// </summary>
        /// <param name="tollEntries">A list of toll entries</param>
        /// <returns>A list of vehicles with total toll fee and details</returns>
        public List<VehicleFee> Calculate(List<TollEntry> tollEntries)
        {
            var result = tollEntries.GroupBy(e => e.RegNo).Select(v =>
                new VehicleFee
                {
                    RegNo = v.Key,
                    TotalFee = GetVehicleFee(v.Key, v.ToList()),
                    Details = GetDetails(v.Key, v.OrderBy(e => e.EntryTime).ToList())
                }
             ).ToList();

            return result;
        }

        public static IReadOnlyList<(TimeSpan From, TimeSpan To, decimal Fee)> GetFeeSchedule()
        {
            return FeeSchedule;
        }

        private static readonly (TimeSpan From, TimeSpan To, decimal Fee)[] FeeSchedule =
        {
            (new TimeSpan(6, 0, 0),  new TimeSpan(6, 29, 0),  MinFee),
            (new TimeSpan(6, 30, 0), new TimeSpan(6, 59, 0),  13),
            (new TimeSpan(7, 0, 0),  new TimeSpan(7, 59, 0),  MaxFee),
            (new TimeSpan(8, 0, 0),  new TimeSpan(8, 29, 0),  13),
            (new TimeSpan(8, 30, 0), new TimeSpan(14, 59, 0), MinFee),
            (new TimeSpan(15, 0, 0), new TimeSpan(15, 29, 0), 13),
            (new TimeSpan(15, 30, 0),new TimeSpan(16, 59, 0), MaxFee),
            (new TimeSpan(17, 0, 0), new TimeSpan(17, 59, 0), 13),
            (new TimeSpan(18, 0, 0), new TimeSpan(18, 29, 0), MinFee),
        };

        private decimal GetVehicleFee(string regNo, List<TollEntry> entries)
        {
            var vehicle = _vehicleRegistry.GetVehicle(regNo);
            decimal totalFee = 0;

            if (IsVehicleTollFree(vehicle))
                return 0;

            var chargeableEntries = entries
                .Where(e => !IsTollFreeDate(e.EntryTime))
                .OrderBy(e => e.EntryTime)
                .ToList();

            DateTime windowStart = DateTime.MinValue;
            decimal windowMaxFee = 0;

            foreach (TollEntry te in chargeableEntries)
            {
                decimal fee = GetFeeForTime(te.EntryTime);

                if (te.EntryTime >= windowStart.AddMinutes(60))
                {
                    // Outside current window — add previous window fee and start new window
                    totalFee += windowMaxFee;
                    windowStart = te.EntryTime;
                    windowMaxFee = fee;
                }
                else
                {
                    // Inside current window — keep track of highest fee
                    windowMaxFee = Math.Max(windowMaxFee, fee);
                }
            }

            totalFee += windowMaxFee;

            return Math.Min(totalFee, DailyFeeCap);
        }

        private bool IsVehicleTollFree(Vehicle vehicle)
        {
            return vehicle is Buss;
        }

        private decimal GetFeeForTime(DateTime date)
        {
            var time = date.TimeOfDay;
            foreach (var (from, to, fee) in FeeSchedule)
            {
                if (time >= from && time <= to)
                    return fee;
            }
            return 0;
        }

        private bool IsTollFreeDate(DateTime date)
        {
            return _swedishHolidayService.IsWeekend(date)
                || _swedishHolidayService.IsPublicHoliday(date);
        }

        private List<VehicleFeeDetails> GetDetails(string regNo, List<TollEntry> entries)
        {
            var vehicle = _vehicleRegistry.GetVehicle(regNo);

            return entries.Select(e => new VehicleFeeDetails
            {
                EntryTime = e.EntryTime,
                Fee = IsVehicleTollFree(vehicle) || IsTollFreeDate(e.EntryTime)
                    ? 0
                    : GetFeeForTime(e.EntryTime)
            }).ToList();
        }
    }
}