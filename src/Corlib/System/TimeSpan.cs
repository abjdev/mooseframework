//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
namespace System
{
    // TimeSpan represents a duration of time.  A TimeSpan can be negative
    // or positive.
    //
    // TimeSpan is internally represented as a number of milliseconds.  While
    // this maps well into units of time such as hours and days, any
    // periods longer than that aren't representable in a nice fashion.
    // For instance, a month can be between 28 and 31 days, while a year
    // can contain 365 or 364 days.  A decade can have between 1 and 3 leapyears,
    // depending on when you map the TimeSpan into the calendar.  This is why
    // we do not provide Years() or Months().
    //
    // Note: System.TimeSpan needs to interop with the WinRT structure
    // type Windows::Foundation:TimeSpan. These types are currently binary-compatible in
    // memory so no custom marshalling is required. If at any point the implementation
    // details of this type should change, or new fields added, we need to remember to add
    // an appropriate custom ILMarshaler to keep WInRT interop scenarios enabled.
    //
    public struct TimeSpan
    {
        public const long TicksPerMillisecond = 10000;
        private const double MillisecondsPerTick = 1.0 / TicksPerMillisecond;

        public const long TicksPerSecond = TicksPerMillisecond * 1000;   // 10,000,000
        private const double SecondsPerTick = 1.0 / TicksPerSecond;         // 0.0001

        public const long TicksPerMinute = TicksPerSecond * 60;         // 600,000,000
        private const double MinutesPerTick = 1.0 / TicksPerMinute; // 1.6666666666667e-9

        public const long TicksPerHour = TicksPerMinute * 60;        // 36,000,000,000
        private const double HoursPerTick = 1.0 / TicksPerHour; // 2.77777777777777778e-11

        public const long TicksPerDay = TicksPerHour * 24;          // 864,000,000,000
        private const double DaysPerTick = 1.0 / TicksPerDay; // 1.1574074074074074074e-12

        private const int MillisPerSecond = 1000;
        private const int MillisPerMinute = MillisPerSecond * 60; //     60,000
        private const int MillisPerHour = MillisPerMinute * 60;   //  3,600,000
        private const int MillisPerDay = MillisPerHour * 24;      // 86,400,000

        internal const long MaxSeconds = long.MaxValue / TicksPerSecond;
        internal const long MinSeconds = long.MinValue / TicksPerSecond;

        internal const long MaxMilliSeconds = long.MaxValue / TicksPerMillisecond;
        internal const long MinMilliSeconds = long.MinValue / TicksPerMillisecond;

        internal const long TicksPerTenthSecond = TicksPerMillisecond * 100;

        public static readonly TimeSpan Zero = new(0);

        public static readonly TimeSpan MaxValue = new(long.MaxValue);
        public static readonly TimeSpan MinValue = new(long.MinValue);

        // internal so that DateTime doesn't have to call an extra get
        // method for some arithmetic operations.
        internal long _ticks;

        //public TimeSpan() {
        //    _ticks = 0;
        //}

        public TimeSpan(long ticks)
        {
            _ticks = ticks;
        }

        public TimeSpan(int hours, int minutes, int seconds)
        {
            _ticks = TimeToTicks(hours, minutes, seconds);
        }

        public TimeSpan(int days, int hours, int minutes, int seconds)
            : this(days, hours, minutes, seconds, 0)
        {
        }

        public TimeSpan(int days, int hours, int minutes, int seconds, int milliseconds)
        {
            long totalMilliSeconds = ((((long)days * 3600 * 24) + ((long)hours * 3600) + ((long)minutes * 60) + seconds) * 1000) + milliseconds;
            if (totalMilliSeconds > MaxMilliSeconds || totalMilliSeconds < MinMilliSeconds)
            {
                //throw new ArgumentOutOfRangeException(null, Environment.GetResourceString("Overflow_TimeSpanTooLong"));
            }

            _ticks = totalMilliSeconds * TicksPerMillisecond;
        }

        public long Ticks => _ticks;

        public int Days => (int)(_ticks / TicksPerDay);

        public int Hours => (int)(_ticks / TicksPerHour % 24);

        public int Milliseconds => (int)(_ticks / TicksPerMillisecond % 1000);

        public int Minutes => (int)(_ticks / TicksPerMinute % 60);

        public int Seconds => (int)(_ticks / TicksPerSecond % 60);

        public double TotalDays => _ticks * DaysPerTick;

        public double TotalHours => _ticks * HoursPerTick;

        public double TotalMilliseconds
        {
            get
            {
                double temp = _ticks * MillisecondsPerTick;
                return temp > MaxMilliSeconds ? MaxMilliSeconds : temp < MinMilliSeconds ? MinMilliSeconds : temp;
            }
        }

        public double TotalMinutes => _ticks * MinutesPerTick;

        public double TotalSeconds => _ticks * SecondsPerTick;

        public TimeSpan Add(TimeSpan ts)
        {
            long result = _ticks + ts._ticks;
            // Overflow if signs of operands was identical and result's
            // sign was opposite.
            // >> 63 gives the sign bit (either 64 1's or 64 0's).
            return (_ticks >> 63 == ts._ticks >> 63) && (_ticks >> 63 != result >> 63)
                ? new TimeSpan()//throw new OverflowException(Environment.GetResourceString("Overflow_TimeSpanTooLong"))
                : new TimeSpan(result);
        }


        // Compares two TimeSpan values, returning an integer that indicates their
        // relationship.
        //
        public static int Compare(TimeSpan t1, TimeSpan t2)
        {
            return t1._ticks > t2._ticks ? 1 : t1._ticks < t2._ticks ? -1 : 0;
        }

        // Returns a value less than zero if this  object
        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }

            if (!(value is TimeSpan))
            {
                //throw new ArgumentException(Environment.GetResourceString("Arg_MustBeTimeSpan"));
            }

            long t = ((TimeSpan)value)._ticks;
            return _ticks > t ? 1 : _ticks < t ? -1 : 0;
        }

        public static TimeSpan FromDays(double value)
        {
            return Interval(value, MillisPerDay);
        }

        public TimeSpan Duration()
        {
            if (Ticks == TimeSpan.MinValue.Ticks)
            {
                //throw new OverflowException(Environment.GetResourceString("Overflow_Duration"));
            }
            return new TimeSpan(_ticks >= 0 ? _ticks : -_ticks);
        }

        public override bool Equals(object value)
        {
            return value is TimeSpan && _ticks == ((TimeSpan)value)._ticks;
        }

        public bool Equals(TimeSpan obj)
        {
            return _ticks == obj._ticks;
        }

        public static bool Equals(TimeSpan t1, TimeSpan t2)
        {
            return t1._ticks == t2._ticks;
        }

        public override int GetHashCode()
        {
            return (int)_ticks ^ (int)(_ticks >> 32);
        }

        public static TimeSpan FromHours(double value)
        {
            return Interval(value, MillisPerHour);
        }

        private static TimeSpan Interval(double value, int scale)
        {
            if (double.IsNaN(value))
            {
                //throw new ArgumentException(Environment.GetResourceString("Arg_CannotBeNaN"));
            }

            double tmp = value * scale;
            double millis = tmp + (value >= 0 ? 0.5 : -0.5);
            return (millis > long.MaxValue / TicksPerMillisecond) || (millis < long.MinValue / TicksPerMillisecond)
                ? new TimeSpan()// throw new OverflowException(Environment.GetResourceString("Overflow_TimeSpanTooLong"))
                : new TimeSpan((long)millis * TicksPerMillisecond);
        }

        public static TimeSpan FromMilliseconds(double value)
        {
            return Interval(value, 1);
        }

        public static TimeSpan FromMinutes(double value)
        {
            return Interval(value, MillisPerMinute);
        }

        public TimeSpan Negate()
        {
            if (Ticks == TimeSpan.MinValue.Ticks)
            {
                //throw new OverflowException(Environment.GetResourceString("Overflow_NegateTwosCompNum"));
            }
            return new TimeSpan(-_ticks);
        }

        public static TimeSpan FromSeconds(double value)
        {
            return Interval(value, MillisPerSecond);
        }

        public TimeSpan Subtract(TimeSpan ts)
        {
            long result = _ticks - ts._ticks;
            // Overflow if signs of operands was different and result's
            // sign was opposite from the first argument's sign.
            // >> 63 gives the sign bit (either 64 1's or 64 0's).
            return (_ticks >> 63 != ts._ticks >> 63) && (_ticks >> 63 != result >> 63)
                ? new TimeSpan()//throw new OverflowException(Environment.GetResourceString("Overflow_TimeSpanTooLong"))
                : new TimeSpan(result);
        }

        public static TimeSpan FromTicks(long value)
        {
            return new TimeSpan(value);
        }

        internal static long TimeToTicks(int hour, int minute, int second)
        {
            // totalSeconds is bounded by 2^31 * 2^12 + 2^31 * 2^8 + 2^31,
            // which is less than 2^44, meaning we won't overflow totalSeconds.
            long totalSeconds = ((long)hour * 3600) + ((long)minute * 60) + second;
            return totalSeconds > MaxSeconds || totalSeconds < MinSeconds
                ? 0// throw new ArgumentOutOfRangeException(null, Environment.GetResourceString("Overflow_TimeSpanTooLong"))
                : totalSeconds * TicksPerSecond;
        }

        public static TimeSpan operator -(TimeSpan t)
        {
            return t._ticks == TimeSpan.MinValue._ticks
                ? new TimeSpan()// throw new OverflowException(Environment.GetResourceString("Overflow_NegateTwosCompNum"))
                : new TimeSpan(-t._ticks);
        }

        public static TimeSpan operator -(TimeSpan t1, TimeSpan t2)
        {
            return t1.Subtract(t2);
        }

        public static TimeSpan operator +(TimeSpan t)
        {
            return t;
        }

        public static TimeSpan operator +(TimeSpan t1, TimeSpan t2)
        {
            return t1.Add(t2);
        }

        public static bool operator ==(TimeSpan t1, TimeSpan t2)
        {
            return t1._ticks == t2._ticks;
        }

        public static bool operator !=(TimeSpan t1, TimeSpan t2)
        {
            return t1._ticks != t2._ticks;
        }

        public static bool operator <(TimeSpan t1, TimeSpan t2)
        {
            return t1._ticks < t2._ticks;
        }

        public static bool operator <=(TimeSpan t1, TimeSpan t2)
        {
            return t1._ticks <= t2._ticks;
        }

        public static bool operator >(TimeSpan t1, TimeSpan t2)
        {
            return t1._ticks > t2._ticks;
        }

        public static bool operator >=(TimeSpan t1, TimeSpan t2)
        {
            return t1._ticks >= t2._ticks;
        }
    }
}