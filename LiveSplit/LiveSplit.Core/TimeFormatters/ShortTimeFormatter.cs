﻿using System;
using System.Globalization;

namespace LiveSplit.TimeFormatters
{
    public class ShortTimeFormatter : GeneralTimeFormatter
    {
        public ShortTimeFormatter()
        {
            Accuracy = TimeAccuracy.Hundredths;
            NullFormat = NullFormat.ZeroWithAccuracy;
        }

        public string Format(TimeSpan? time, TimeFormat format)
        {
            var formatRequest = new GeneralTimeFormatter
            {
                Accuracy = TimeAccuracy.Hundredths,
                NullFormat = NullFormat.ZeroWithAccuracy,
                TimeFormat = format
            };

            return formatRequest.Format(time);
        }
        public string Format(TimeSpan? time, TimeFormat format, TimeSystem system)
        {
            var formatRequest = new GeneralTimeFormatter
            {
                Accuracy = TimeAccuracy.Hundredths,
                NullFormat = NullFormat.ZeroWithAccuracy,
                TimeFormat = format,
                TimeSystem = system
            };

            return formatRequest.Format(time);
        }
    }
}
