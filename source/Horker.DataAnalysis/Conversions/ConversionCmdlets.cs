using System;
using System.Management.Automation;

namespace Horker.DataAnalysis
{
    [Cmdlet("ConvertTo", "Math.Double")]
    [Alias("math.todouble")]
    public class ConvertToMathDouble : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public object InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public object[] Values;

        protected override void ProcessRecord()
        {
            if (InputObject != null)
                WriteObject(Converter.ToDouble(InputObject));
        }

        protected override void EndProcessing()
        {
            foreach (var value in Values)
            {
                WriteObject(Converter.ToDouble(value));
            }
        }
    }

    [Cmdlet("ConvertTo", "Math.DateTime")]
    [Alias("math.todatetime")]
    public class ConvertToMathDateTime : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public object InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public object[] Values;

        [Parameter(Position = 1, Mandatory = false)]
        public string Format;

        private DateTime? Convert(object value)
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

    [Cmdlet("ConvertTo", "Math.DateTimeOffset")]
    [Alias("math.todatetimeoffset")]
    public class ConvertToMathDateTimeOffset : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public object InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public object[] Values;

        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "Format")]
        public string Format;

        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "Universal")]
        public SwitchParameter AssumeUniversal;

        private DateTimeOffset? Convert(object value)
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
}
