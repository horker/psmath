using System;
using System.Management.Automation;

namespace Horker.DataAnalysis
{
    public abstract class ConversionCmdletBase<T> : PSCmdlet
    {
        protected static DateTime EpochInDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        protected static DateTimeOffset EpochInDateTimeOffset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public object InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public object[] Values;

        protected abstract T Convert(object value);

        protected override void ProcessRecord()
        {
            if (InputObject != null)
                WriteObject(Convert(InputObject));
        }

        protected override void EndProcessing()
        {
            foreach (var value in Values)
            {
                WriteObject(Convert(value));
            }
        }
    }

    [Cmdlet("ConvertTo", "Math.Double")]
    [Alias("math.todouble")]
    public class ConvertToMathDouble : ConversionCmdletBase<double>
    {
        protected override double Convert(object value)
        {
            return Converter.ToDouble(value);
        }
    }

    [Cmdlet("ConvertTo", "Math.DateTime")]
    [Alias("math.todatetime")]
    public class ConvertToMathDateTime : ConversionCmdletBase<DateTime?>
    {
        [Parameter(Position = 1, Mandatory = false)]
        public string Format;

        protected override DateTime? Convert(object value)
        {
            if (String.IsNullOrEmpty(Format))
            {
                return Converter.ToDateTime(value);
            }
            else
            {
                return Converter.ToDateTime(value, Format);
            }
        }
    }

    [Cmdlet("ConvertTo", "Math.DateTimeOffset")]
    [Alias("math.todatetimeoffset")]
    public class ConvertToMathDateTimeOffset : ConversionCmdletBase<DateTimeOffset?>
    {
        [Parameter(Position = 1, Mandatory = false)]
        public string Format;

        [Parameter(Position = 2, Mandatory = false)]
        public SwitchParameter AssumeUniversal;

        protected override DateTimeOffset? Convert(object value)
        {
            if (String.IsNullOrEmpty(Format))
            {
                return Converter.ToDateTimeOffset(value, !AssumeUniversal);
            }
            else
            {
                return Converter.ToDateTimeOffset(value, Format);
            }
        }
    }

    [Cmdlet("ConvertTo", "Math.UnixTime")]
    [Alias("math.tounixtime")]
    public class ConvertToMathUnixTime : ConversionCmdletBase<double>
    {
        [Parameter(Position = 1, Mandatory = false)]
        public string Format;

        [Parameter(Position = 2, Mandatory = false)]
        public SwitchParameter AssumeUniversal;

        private DateTimeOffset? ConvertDateTimeOffset(object value)
        {
            if (String.IsNullOrEmpty(Format))
            {
                return Converter.ToDateTimeOffset(value, !AssumeUniversal);
            }
            else
            {
                return Converter.ToDateTimeOffset(value, Format);
            }
        }

        protected override double Convert(object value)
        {
            if (value is PSObject)
            {
                value = (value as PSObject).BaseObject;
            }

            if (value is DateTime)
            {
                return ((DateTime)value - EpochInDateTime).TotalSeconds;
            }
            else if (value is DateTimeOffset)
            {
                return ((DateTimeOffset)value - EpochInDateTime).TotalSeconds;
            }
            else
            {
                var date = ConvertDateTimeOffset(value);
                if (!date.HasValue)
                {
                    return double.NaN;
                }
                return (date.Value - EpochInDateTimeOffset).TotalSeconds;
            }
        }
    }

    [Cmdlet("ConvertFrom", "Math.UnixTime")]
    [Alias("math.fromunixtime")]
    public class ConvertFromMathUnixTime : ConversionCmdletBase<object>
    {
        [Parameter(Position = 1, Mandatory = false)]
        public SwitchParameter DateTime;

        protected override object Convert(object value)
        {
            var offset = Converter.ToDouble(value);
            var date = EpochInDateTimeOffset.AddSeconds(offset);

            if (DateTime)
                return date.LocalDateTime;

            return date;
        }
    }
}
