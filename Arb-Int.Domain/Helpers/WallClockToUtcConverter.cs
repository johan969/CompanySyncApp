
namespace Arb_Int.Domain.Helpers
{
    public enum AmbiguousResolution
    {
        ChooseEarlier = 0, // första förekomsten (ofta DST-offset, t.ex. +02)
        ChooseLater = 1  // andra förekomsten (ofta standardtid, t.ex. +01)
    }

    public sealed class TimeResolutionOptions
    {
        public AmbiguousResolution Ambiguous { get; init; } = AmbiguousResolution.ChooseLater;

        /// <summary>
        /// Hur långt vi max får flytta fram en invalid tid (DST-gap är normalt 60 min).
        /// </summary>
        public TimeSpan MaxInvalidShift { get; init; } = TimeSpan.FromHours(3);

        /// <summary>
        /// Stegstorlek när vi flyttar fram invalid tid. 1 minut är säkrast.
        /// </summary>
        public TimeSpan InvalidShiftStep { get; init; } = TimeSpan.FromMinutes(1);
    }

    public static class WallClockToUtcConverter
    {
        public static DateTimeOffset ConvertLocalToUtcAuto(DateTime localWallClock, TimeZoneInfo tz, TimeResolutionOptions? options = null)
        {
            options ??= new TimeResolutionOptions();

            if (localWallClock.Kind != DateTimeKind.Unspecified)
                localWallClock = DateTime.SpecifyKind(localWallClock, DateTimeKind.Unspecified);

            if (tz.IsInvalidTime(localWallClock))
            {
                var shifted = localWallClock;
                var max = localWallClock + options.MaxInvalidShift;

                while (tz.IsInvalidTime(shifted))
                {
                    shifted = shifted.Add(options.InvalidShiftStep);
                    if (shifted > max)
                        throw new InvalidOperationException($"Could not resolve invalid time '{localWallClock:O}' in '{tz.Id}'.");
                }

                localWallClock = shifted;
            }

            if (tz.IsAmbiguousTime(localWallClock))
            {
                var offsets = tz.GetAmbiguousTimeOffsets(localWallClock);
                var chosen = options.Ambiguous == AmbiguousResolution.ChooseEarlier
                    ? offsets.Max()
                    : offsets.Min();

                return new DateTimeOffset(localWallClock, chosen).ToUniversalTime();
            }

            var offset = tz.GetUtcOffset(localWallClock);
            return new DateTimeOffset(localWallClock, offset).ToUniversalTime();
        }
    }

}
