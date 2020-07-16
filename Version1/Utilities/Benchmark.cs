using System;

namespace Version1.Utilities
{
    public sealed class Benchmark
    {
        private readonly DateTime _startTime = DateTime.Now;

        public string GetElapsedTime()
        {
            DateTime stopTime = DateTime.Now;
            var ts = new TimeSpan(stopTime.Ticks - _startTime.Ticks);
            return ToHumanReadable(ts);
        }

        private static string ToHumanReadable(TimeSpan span)
        {
            return span.Days > 0
                ? $"{span.Days}:{span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}.{span.Milliseconds/100:D1}"
                : $"{span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}.{span.Milliseconds/100:D1}";
        }
    }
}