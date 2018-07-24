using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Horker.Math.PSObjects
{
    [Cmdlet("Get", "PSObject.Summary")]
    [Alias("pso.summary")]
    public class GetPSObjectSummary : ObjectListCmdletBase<PSObject>
    {
        protected override void Process(IReadOnlyList<PSObject> data)
        {
            var propNames = new List<string>();
            var elements = new Dictionary<string, List<object>>();

            foreach (var row in data)
            {
                foreach (var prop in row.Properties)
                {
                    if (!elements.ContainsKey(prop.Name))
                    {
                        propNames.Add(prop.Name);
                        elements.Add(prop.Name, new List<object>());
                    }

                    elements[prop.Name].Add(prop.Value);
                }
            }

            foreach (var name in propNames)
            {
                var result = new DataSummary(elements[name].Select(x => Converter.ToDouble(x)).ToArray(), name);
                WriteObject(result);
            }
        }
    }
}
