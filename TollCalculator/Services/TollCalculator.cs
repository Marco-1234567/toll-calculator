using TollCalculator.Models;

namespace TollCalculator.Services
{
    /// <summary>
    /// Service responsible for calculating toll fees based on Swedish congestion tax rules.
    /// </summary>
    public class TollCalculatorService
    {
        private const decimal DailyFeeCap = 60;
        private const decimal MinFee = 8;
        private const decimal MaxFee = 18;

        private readonly VehicleRegistry _vehicleRegistry;
        private readonly SwedishHolidayService _swedishHolidayService;

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

        /// <summary>
        /// Initializes the toll calculator with required dependencies.
        /// </summary>
        /// <param name="vehicleRegistry">Registry of known vehicles used to look up vehicle types by registration number</param>
        /// <param name="holidayService">Service for determining Swedish public holidays and weekends</param>
        /// <exception cref="ArgumentNullException">Thrown when reg or holidays is null</exception>
        public TollCalculatorService(VehicleRegistry vehicleRegistry, SwedishHolidayService holidayService)
        {
            ArgumentNullException.ThrowIfNull(vehicleRegistry);
            ArgumentNullException.ThrowIfNull(holidayService);

            _vehicleRegistry = vehicleRegistry;
            _swedishHolidayService = holidayService;
        }

        /// <summary>
        /// Calculates total toll fees for a list of toll entries grouped by registration number.
        /// Unknown vehicles are charged full fee and flagged with IsUnknown = true.
        /// A warning is written to Console.Error for each unknown vehicle.
        /// </summary>
        /// <param name="tollEntries">List of toll entries to calculate fees for</param>
        /// <returns>List of vehicle fees grouped by registration number</returns>
        /// <exception cref="ArgumentNullException">Thrown when tollEntries is null</exception>
        public List<VehicleFee> Calculate(List<TollEntry> tollEntries)
        {
            ArgumentNullException.ThrowIfNull(tollEntries);

            var unknowns = GetUnknownVehicles(tollEntries);
            
            foreach (var regNo in unknowns)
                Console.Error.WriteLine($"WARNING: Vehicle {regNo} not found in registry");

            var knownRegNos = tollEntries
                .Select(e => e.RegNo)
                .Where(regNo => _vehicleRegistry.GetVehicle(regNo) != null)
                .Distinct();

            var result = tollEntries.GroupBy(e => e.RegNo).Select(v =>
                new VehicleFee
                {
                    RegNo = v.Key,
                    IsUnknown = _vehicleRegistry.GetVehicle(v.Key) == null,
                    TotalFee = GetVehicleFee(v.Key, v.ToList()),
                    Details = GetDetails(v.Key, v.OrderBy(e => e.EntryTime).ToList())
                }
            ).ToList();

            return result;
        }

        /// <summary>
        /// Returns a list of registration numbers not found in the vehicle registry.
        /// </summary>
        /// <param name="tollEntries">List of toll entries to check</param>
        /// <returns>List of unknown registration numbers</returns>
        public List<string> GetUnknownVehicles(List<TollEntry> tollEntries)
        {
            return tollEntries
                .Select(e => e.RegNo)
                .Distinct()
                .Where(regNo => _vehicleRegistry.GetVehicle(regNo) == null)
                .ToList();
        }

        /// <summary>
        /// Returns the toll fee schedule as a read only list.
        /// </summary>
        /// <returns>Read only list of fee intervals with from, to and fee</returns>
        public static IReadOnlyList<(TimeSpan From, TimeSpan To, decimal Fee)> GetFeeSchedule()
        {
            return FeeSchedule;
        }

        private decimal GetVehicleFee(string regNo, List<TollEntry> entries)
        {
            var vehicle = _vehicleRegistry.GetVehicle(regNo);

            if (vehicle != null && IsVehicleTollFree(vehicle))
                return 0;

            return entries
                .GroupBy(e => e.EntryTime.Date)
                .Sum(dayEntries => GetDailyFee(dayEntries.ToList()));
        }

        private decimal GetDailyFee(List<TollEntry> entries)
        {
            decimal fee = 0;
            DateTime windowStart = DateTime.MinValue;
            decimal windowMaxFee = 0;

            foreach (TollEntry te in entries.OrderBy(e => e.EntryTime))
            {
                if (IsTollFreeDate(te.EntryTime))
                    continue;

                decimal entryFee = GetFeeForTime(te.EntryTime);

                if (te.EntryTime >= windowStart.AddMinutes(60))
                {
                    fee += windowMaxFee;
                    windowStart = te.EntryTime;
                    windowMaxFee = entryFee;
                }
                else
                {
                    windowMaxFee = Math.Max(windowMaxFee, entryFee);
                }
            }

            fee += windowMaxFee;

            return Math.Min(fee, DailyFeeCap);
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
                Fee = (vehicle != null && IsVehicleTollFree(vehicle)) || IsTollFreeDate(e.EntryTime)
                    ? 0
                    : GetFeeForTime(e.EntryTime)
            }).ToList();
        }
    }
}