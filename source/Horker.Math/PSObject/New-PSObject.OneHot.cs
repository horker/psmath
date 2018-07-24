using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace Horker.Math.PSObjects
{
    [Cmdlet("New", "PSObject.OneHot")]
    [Alias("pso.onehot")]
    public class NewPSObjectOneHot : ObjectListCmdletBase<object>
    {
        [Parameter(Position = 1, Mandatory = false)]
        public string[] Categories;

        protected override void Process(IReadOnlyList<object> data)
        {
            string[] categories = Categories;

            if (Categories == null || Categories.Length == 0)
            {
                categories = data.Distinct().Select(x => x.ToString()).ToArray();
                Array.Sort(categories);
            }

            foreach (var d in data)
            {
                var obj = new PSObject();

                foreach (var c in categories)
                {
                    if (c == d.ToString())
                        obj.Properties.Add(new PSNoteProperty(c, 1));
                    else
                        obj.Properties.Add(new PSNoteProperty(c, 0));
                }

                WriteObject(obj);
            }
        }
    }
}
