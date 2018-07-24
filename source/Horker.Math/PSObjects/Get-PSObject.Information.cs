using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Horker.Math.PSObjects
{
    public class Information
    {
        public string Name;
        public int Total;
        public int Unique;
        public int String;
        public int Numeric;
        public int DateTime;
        public int Boolean;
        public int Other;
        public int Null;
    }

    [Cmdlet("Get", "PSObjects.Information")]
    [Alias("pso.info")]
    public class GetPSObjectsInformation : ObjectListCmdletBase<PSObject>
    {
        [Parameter(Position = 1, Mandatory = false)]
        public SwitchParameter NoTypeInference;

        protected override void Process(IReadOnlyList<PSObject> data)
        {
            var propNames = new List<string>();
            var summaries = new Dictionary<string, Information>();
            var elements = new Dictionary<string, List<object>>();

            foreach (var row in data)
            {
                foreach (var prop in row.Properties)
                {
                    if (!summaries.ContainsKey(prop.Name))
                    {
                        propNames.Add(prop.Name);
                        var s = new Information();
                        s.Name = prop.Name;
                        summaries.Add(prop.Name, s);
                        elements.Add(prop.Name, new List<object>());
                    }

                    var value = prop.Value;
                    elements[prop.Name].Add(value);

                    var summary = summaries[prop.Name];

                    ++summary.Total;

                    if (value is null)
                    {
                        ++summary.Null;
                    }
                    else if (value is Boolean)
                    {
                        ++summary.Boolean;
                    }
                    else if (value is Double || value is Single)
                    {
                        ++summary.Numeric;
                        if (double.IsNaN((Double)value))
                            ++summary.Null;
                    }
                    else if (value is Int64 || value is Int32 || value is Byte || value is SByte)
                    {
                        ++summary.Numeric;
                    }
                    else if (value is DateTime || value is DateTimeOffset)
                    {
                        ++summary.DateTime;
                    }
                    else if (value is String)
                    {
                        var s = value as string;
                        if (string.IsNullOrWhiteSpace(s))
                        {
                            ++summary.String;
                            ++summary.Null;
                        }
                        else
                        {
                            if (NoTypeInference)
                                ++summary.String;
                            else
                            {
                                bool numeric = false;
                                bool boolean = false;
                                bool dateTime = false;

                                s = s.Trim().ToLower();

                                if (s == "true" || s == "t" || s == "false" || s == "f")
                                    boolean = true;
                                else
                                {
                                    if (s != "nan")
                                    {
                                        var converted = Converter.ToDouble(value);
                                        if (!double.IsNaN(converted))
                                            numeric = true;
                                    }

                                    if (!numeric)
                                    {
                                        var converted = Converter.ToDateTimeOffset(value);
                                        if (converted.HasValue)
                                            dateTime = true;
                                    }
                                }

                                if (numeric)
                                    ++summary.Numeric;
                                else if (boolean)
                                    ++summary.Boolean;
                                else if (dateTime)
                                    ++summary.DateTime;
                                else
                                    ++summary.String;
                            }
                        }
                    }
                }
            }

            foreach (var name in propNames)
            {
                var summary = summaries[name];

                summary.Unique = elements[name].Distinct().Count();
                summary.Other = summary.Total - (summary.String + summary.Numeric + summary.DateTime + summary.Boolean);

                WriteObject(summary);
            }
        }
    }
}
