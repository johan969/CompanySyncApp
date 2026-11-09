
using Arb_Int.Domain.Helpers;

namespace Arb_Int.Domain.Worker
{
    public static class ScheduleCalculator
    {

        public static DateTimeOffset ComputeNextRunAtUtcAuto(
        ScheduleTriggerType triggerType,
        string timeZoneId,
        DateTimeOffset lastRunAtUtc,
        TimeResolutionOptions? options = null)
        {
            if (triggerType == ScheduleTriggerType.None)
                throw new InvalidOperationException("TriggerType=None has no schedule.");

            if (triggerType == ScheduleTriggerType.Once)
                throw new InvalidOperationException("TriggerType=Once has no next occurrence.");

            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            // last utc -> local wall clock (Unspecified)
            var lastLocalDto = TimeZoneInfo.ConvertTime(lastRunAtUtc, tz);
            var lastLocal = DateTime.SpecifyKind(lastLocalDto.DateTime, DateTimeKind.Unspecified);

            var nextLocal = ComputeNextLocalWallClock(triggerType, lastLocal);

            // AUTO: fix invalid + resolve ambiguous
            return WallClockToUtcConverter.ConvertLocalToUtcAuto(nextLocal, tz, options);
        }
        private static DateTime ComputeNextLocalWallClock(ScheduleTriggerType triggerType, DateTime lastLocal)
        {
            return triggerType switch
            {
                // ---- minute/hour-based ----
                ScheduleTriggerType.EveryMinute => lastLocal.AddMinutes(1),
                ScheduleTriggerType.EveryFiveMinutes => lastLocal.AddMinutes(5),
                ScheduleTriggerType.EveryTenMinutes => lastLocal.AddMinutes(10),
                ScheduleTriggerType.EveryFifteenMinutes => lastLocal.AddMinutes(15),
                ScheduleTriggerType.EveryTwentyMinutes => lastLocal.AddMinutes(20),
                ScheduleTriggerType.EveryHalfHour => lastLocal.AddMinutes(30),
                ScheduleTriggerType.EvertFourtyFiveMinutes => lastLocal.AddMinutes(45),
                ScheduleTriggerType.EveryHour => lastLocal.AddHours(1),
                ScheduleTriggerType.EveryTwoHours => lastLocal.AddHours(2),
                ScheduleTriggerType.EveryThreeHours => lastLocal.AddHours(3),
                ScheduleTriggerType.EverySixHours => lastLocal.AddHours(6),
                ScheduleTriggerType.EveryTwelveHours => lastLocal.AddHours(12),

                // ---- daily/weekly ----
                ScheduleTriggerType.Daily => lastLocal.AddDays(1),
                ScheduleTriggerType.Weekly => lastLocal.AddDays(7),

                // ---- weekday-specific ----
                ScheduleTriggerType.EveryMonday => NextDow(lastLocal, DayOfWeek.Monday),
                ScheduleTriggerType.EveryTuesday => NextDow(lastLocal, DayOfWeek.Tuesday),
                ScheduleTriggerType.EveryWednesday => NextDow(lastLocal, DayOfWeek.Wednesday),
                ScheduleTriggerType.EveryThirsday => NextDow(lastLocal, DayOfWeek.Thursday),
                ScheduleTriggerType.EveryFriday => NextDow(lastLocal, DayOfWeek.Friday),
                ScheduleTriggerType.EverySaturday => NextDow(lastLocal, DayOfWeek.Saturday),
                ScheduleTriggerType.EverySunday => NextDow(lastLocal, DayOfWeek.Sunday),

                ScheduleTriggerType.EveryWeekday => NextWeekday(lastLocal),

                // ---- month/quarter/half-year/year ----
                ScheduleTriggerType.Monthly => lastLocal.AddMonths(1),
                ScheduleTriggerType.Quarterly => lastLocal.AddMonths(3),
                ScheduleTriggerType.HalfAYear => lastLocal.AddMonths(6),
                ScheduleTriggerType.Yearly => lastLocal.AddYears(1),

                // ---- "day-of-month" variants ----
                ScheduleTriggerType.DayTheMonth1st => NextDayOfMonth(lastLocal, 1),
                ScheduleTriggerType.DayfTheMonth2nd => NextDayOfMonth(lastLocal, 2),
                ScheduleTriggerType.DayfTheMonth3rd => NextDayOfMonth(lastLocal, 3),
                ScheduleTriggerType.DayfTheMonth4th => NextDayOfMonth(lastLocal, 4),
                ScheduleTriggerType.DayfTheMonth5th => NextDayOfMonth(lastLocal, 5),
                ScheduleTriggerType.DayfTheMonth6th => NextDayOfMonth(lastLocal, 6),
                ScheduleTriggerType.DayfTheMonth7th => NextDayOfMonth(lastLocal, 7),
                ScheduleTriggerType.DayfTheMonth8th => NextDayOfMonth(lastLocal, 8),
                ScheduleTriggerType.DayfTheMonth9th => NextDayOfMonth(lastLocal, 9),
                ScheduleTriggerType.DayfTheMonth10th => NextDayOfMonth(lastLocal, 10),
                ScheduleTriggerType.DayfTheMonth11th => NextDayOfMonth(lastLocal, 11),
                ScheduleTriggerType.DayfTheMonth12th => NextDayOfMonth(lastLocal, 12),
                ScheduleTriggerType.DayfTheMonth13th => NextDayOfMonth(lastLocal, 13),
                ScheduleTriggerType.DayfTheMonth14th => NextDayOfMonth(lastLocal, 14),
                ScheduleTriggerType.DayfTheMonth15th => NextDayOfMonth(lastLocal, 15),
                ScheduleTriggerType.DayfTheMonth16th => NextDayOfMonth(lastLocal, 16),
                ScheduleTriggerType.DayfTheMonth17th => NextDayOfMonth(lastLocal, 17),
                ScheduleTriggerType.DayfTheMonth18th => NextDayOfMonth(lastLocal, 18),
                ScheduleTriggerType.DayfTheMonth19th => NextDayOfMonth(lastLocal, 19),
                ScheduleTriggerType.DayfTheMonth20th => NextDayOfMonth(lastLocal, 20),
                ScheduleTriggerType.DayfTheMonth21th => NextDayOfMonth(lastLocal, 21),
                ScheduleTriggerType.DayfTheMonth22th => NextDayOfMonth(lastLocal, 22),
                ScheduleTriggerType.DayfTheMonth23th => NextDayOfMonth(lastLocal, 23),
                ScheduleTriggerType.DayfTheMonth24th => NextDayOfMonth(lastLocal, 24),
                ScheduleTriggerType.DayfTheMonth25th => NextDayOfMonth(lastLocal, 25),
                ScheduleTriggerType.DayfTheMonth26th => NextDayOfMonth(lastLocal, 26),
                ScheduleTriggerType.DayfTheMonth27th => NextDayOfMonth(lastLocal, 27),
                ScheduleTriggerType.DayfTheMonth28th => NextDayOfMonth(lastLocal, 28),
                ScheduleTriggerType.DayfTheMonth29th => NextDayOfMonth(lastLocal, 29),
                ScheduleTriggerType.DayfTheMonth30th => NextDayOfMonth(lastLocal, 30),
                ScheduleTriggerType.DayfTheMonth31th => NextDayOfMonth(lastLocal, 31),

                _ => throw new NotSupportedException($"Unsupported trigger type: {triggerType}")
            };
        }

        private static DateTime NextDow(DateTime fromLocal, DayOfWeek target)
        {
            var daysAhead = ((int)target - (int)fromLocal.DayOfWeek + 7) % 7;
            if (daysAhead == 0) daysAhead = 7; // nästa vecka
            return fromLocal.AddDays(daysAhead);
        }

        private static DateTime NextWeekday(DateTime fromLocal)
        {
            var next = fromLocal.AddDays(1);
            while (next.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                next = next.AddDays(1);
            return next;
        }

        private static DateTime NextDayOfMonth(DateTime fromLocal, int day)
        {
            // behåll klockslag
            var time = fromLocal.TimeOfDay;

            // börja i nästa månad om dagens datum redan passerat eller är samma dag (vi vill "nästa occurrence")
            var year = fromLocal.Year;
            var month = fromLocal.Month;

            // Först: försök i denna månad men "nästa gång" => om day <= dagens dag, hoppa månad
            if (day <= fromLocal.Day)
            {
                var nextMonth = fromLocal.AddMonths(1);
                year = nextMonth.Year;
                month = nextMonth.Month;
            }

            // Clamp dag till sista dagen i månaden (29/30/31 special)
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var actualDay = Math.Min(day, daysInMonth);

            return new DateTime(year, month, actualDay, 0, 0, 0, DateTimeKind.Unspecified).Add(time);
        }
    }
}