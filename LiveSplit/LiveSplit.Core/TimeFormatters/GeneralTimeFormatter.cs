using System;
using System.Globalization;

namespace LiveSplit.TimeFormatters {
    public class GeneralTimeFormatter : ITimeFormatter {
        static readonly private CultureInfo ic = CultureInfo.InvariantCulture;

        public TimeAccuracy Accuracy { get; set; }

        [Obsolete("Use DigitsFormat instead")]
        public TimeFormat TimeFormat { set => DigitsFormat = value.ToDigitsFormat(); }

        public DigitsFormat DigitsFormat { get; set; }
        public TimeSystem TimeSystem { get; set; }

        /// <summary>
        /// How to display null times
        /// </summary>
        public NullFormat NullFormat { get; set; }

        /// <summary>
        /// If true, for example show "1d 23:59:10" instead of "47:59:10". For durations of 24 hours or more, 
        /// </summary>
        public bool ShowDays { get; set; }

        /// <summary>
        /// If true, include a "+" for positive times (excluding zero)
        /// </summary>
        public bool ShowPlus { get; set; }

        /// <summary>
        /// If true, don't display decimals if absolute time is 1 minute or more
        /// </summary>
        public bool DropDecimals { get; set; }

        /// <summary>
        /// If true, don't display trailing zero demical places
        /// </summary>
        public bool AutomaticPrecision { get; set; } = false;

        public GeneralTimeFormatter()
        {
            DigitsFormat = DigitsFormat.SingleDigitSeconds;
            NullFormat = NullFormat.Dash;
            TimeSystem = TimeSystem.Standard;
        }

        public string Format(TimeSpan? timeNullable)
        {
            bool isNull = (!timeNullable.HasValue);
            if (isNull) {
                if (NullFormat == NullFormat.Dash) {
                    return TimeFormatConstants.DASH;
                } else if (NullFormat == NullFormat.ZeroWithAccuracy) {
                    return ZeroWithAccuracy();
                } else if (NullFormat == NullFormat.ZeroDotZeroZero) {
                    return "0.00";
                } else if (NullFormat == NullFormat.ZeroValue || NullFormat == NullFormat.Dashes) {
                    timeNullable = TimeSpan.Zero;
                }
            }

            TimeSpan time = timeNullable.Value;

            string minusString;
            if (time < TimeSpan.Zero)
            {
                minusString = TimeFormatConstants.MINUS;
                time = -time;
            }
            else
            {
                minusString = (ShowPlus ? "+" : "");
            }

            if (TimeSystem == TimeSystem.Decimal)
            {
                var TotalDecimalMilliSeconds = (long) time.TotalMilliseconds / 0.864;
                var TotalDecimalSeconds = (long)TotalDecimalMilliSeconds / 1000;
                var TotalDecimalMinutes = TotalDecimalSeconds / 100;
                var TotalDecimalHours = TotalDecimalMinutes / 100;

                var DecimalMilliSeconds = (long) TotalDecimalMilliSeconds % 1000;
                var DecimalSeconds = TotalDecimalSeconds % 100;
                var DecimalMinutes = TotalDecimalMinutes % 100;

                string DecimalMilliSecondsFormated = "";
                if (AutomaticPrecision)
                {
                    if (Accuracy == TimeAccuracy.Seconds || TotalDecimalSeconds % 1 == 0)
                        DecimalMilliSecondsFormated = "";
                    else if (Accuracy == TimeAccuracy.Tenths || (10 * TotalDecimalSeconds) % 1 == 0)
                        DecimalMilliSecondsFormated = $".{DecimalMilliSeconds/100}";
                    else if (Accuracy == TimeAccuracy.Hundredths || (100 * TotalDecimalSeconds) % 1 == 0)
                        DecimalMilliSecondsFormated = $".{DecimalMilliSeconds/10:D2}";
                    else
                        DecimalMilliSecondsFormated = $".{DecimalMilliSeconds:D3}";
                }
                else
                {
                    if (DropDecimals && time.TotalMinutes >= 1)
                        DecimalMilliSecondsFormated = "";
                    else if (Accuracy == TimeAccuracy.Seconds)
                        DecimalMilliSecondsFormated = "";
                    else if (Accuracy == TimeAccuracy.Tenths)
                        DecimalMilliSecondsFormated = $".{DecimalMilliSeconds/100}";
                    else if (Accuracy == TimeAccuracy.Hundredths)
                        DecimalMilliSecondsFormated = $".{DecimalMilliSeconds/10:D2}";
                    else if (Accuracy == TimeAccuracy.Milliseconds)
                        DecimalMilliSecondsFormated = $".{DecimalMilliSeconds:D3}";
                }

                string timeFormated = "";
                if (ShowDays && time.TotalDays>0)
                {
                    var DecimalHours = TotalDecimalHours % 10;
                    timeFormated = $"{time.TotalDays}:{DecimalHours}:{DecimalMinutes:D2}:{DecimalSeconds:D2}{DecimalMilliSecondsFormated}";
                }
                else if (TotalDecimalHours > 0 || DigitsFormat == DigitsFormat.DoubleDigitHours || DigitsFormat == DigitsFormat.SingleDigitHours)
                {
                    timeFormated = $"{TotalDecimalHours}:{DecimalMinutes:D2}:{DecimalSeconds:D2}{DecimalMilliSecondsFormated}";
                }
                else if (DecimalMinutes > 9 || DigitsFormat == DigitsFormat.DoubleDigitMinutes)
                {
                    timeFormated = $"{DecimalMinutes:D2}:{DecimalSeconds:D2}{DecimalMilliSecondsFormated}";
                }
                else if (DecimalMinutes > 0 || DigitsFormat == DigitsFormat.SingleDigitMinutes)
                {
                    timeFormated = $"{DecimalMinutes:D1}:{DecimalSeconds:D2}{DecimalMilliSecondsFormated}";
                }
                else if (DecimalSeconds > 9 || DigitsFormat == DigitsFormat.DoubleDigitSeconds)
                {
                    timeFormated = $"{DecimalSeconds:D2}{DecimalMilliSecondsFormated}";
                }
                else
                {
                    timeFormated = $"{DecimalSeconds:D1}{DecimalMilliSecondsFormated}";
                }
                return minusString + timeFormated;
            }

            string decimalFormat = "";
            if (AutomaticPrecision)
            {
                var totalSeconds = time.TotalSeconds;
                if (Accuracy == TimeAccuracy.Seconds || totalSeconds % 1 == 0)
                    decimalFormat = "";
                else if (Accuracy == TimeAccuracy.Tenths || (10 * totalSeconds) % 1 == 0)
                    decimalFormat = @"\.f";
                else if (Accuracy == TimeAccuracy.Hundredths || (100 * totalSeconds) % 1 == 0)
                    decimalFormat = @"\.ff";
                else
                    decimalFormat = @"\.fff";
            }
            else
            {
                if (DropDecimals && time.TotalMinutes >= 1)
                    decimalFormat = "";
                else if (Accuracy == TimeAccuracy.Seconds)
                    decimalFormat = "";
                else if (Accuracy == TimeAccuracy.Tenths)
                    decimalFormat = @"\.f";
                else if (Accuracy == TimeAccuracy.Hundredths)
                    decimalFormat = @"\.ff";
                else if (Accuracy == TimeAccuracy.Milliseconds)
                    decimalFormat = @"\.fff";
            }

            string formatted;
            if (time.TotalDays >= 1)
            {
                if (ShowDays)
                    formatted = minusString + time.ToString(@"d\d\ " + (DigitsFormat == DigitsFormat.DoubleDigitHours ? "hh" : "h") + @"\:mm\:ss" + decimalFormat, ic);
                else
                    formatted = minusString + (int)time.TotalHours + time.ToString(@"\:mm\:ss" + decimalFormat, ic);
            }
            else if (DigitsFormat == DigitsFormat.DoubleDigitHours)
            {
                formatted = minusString + time.ToString(@"hh\:mm\:ss" + decimalFormat, ic);
            }
            else if (time.TotalHours >= 1 || DigitsFormat == DigitsFormat.SingleDigitHours)
            {
                formatted = minusString + time.ToString(@"h\:mm\:ss" + decimalFormat, ic);
            }
            else if (DigitsFormat == DigitsFormat.DoubleDigitMinutes)
            {
                formatted = minusString + time.ToString(@"mm\:ss" + decimalFormat, ic);
            }
            else if (time.TotalMinutes >= 1 || DigitsFormat == DigitsFormat.SingleDigitMinutes)
            {
                formatted = minusString + time.ToString(@"m\:ss" + decimalFormat, ic);
            }
            else if (DigitsFormat == DigitsFormat.DoubleDigitSeconds)
            {
                formatted = minusString + time.ToString(@"ss" + decimalFormat, ic);
            }
            else
            {
                formatted = minusString + time.ToString(@"%s" + decimalFormat, ic);
            }

            if (isNull && NullFormat == NullFormat.Dashes)
                formatted = formatted.Replace('0', '-');

            return formatted;
        }

        private string ZeroWithAccuracy()
        {
            if (AutomaticPrecision || Accuracy == TimeAccuracy.Seconds)
                return "0";
            else if (Accuracy == TimeAccuracy.Tenths)
                return "0.0";
            else if (Accuracy == TimeAccuracy.Milliseconds)
                return "0.000";
            else
                return "0.00";
        }
    }
}