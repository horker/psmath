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
    public class NewPSObjectOneHot : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public object InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public object[] Data;

        [Parameter(Position = 1, Mandatory = false)]
        public string[] Categories;

        private List<object> _inputObjects;

        protected override void BeginProcessing()
        {
            _inputObjects = new List<object>();
        }

        protected override void ProcessRecord()
        {
            _inputObjects.Add(InputObject);
        }

        protected override void EndProcessing()
        {
            IReadOnlyList<object> data;

            if (_inputObjects.Count == 0 || (_inputObjects.Count == 1 && _inputObjects[0] == null))
            {
                if (Data == null || Data.Length == 0)
                {
                    WriteError(new ErrorRecord(new ArgumentException("No input specified"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }
                data = Data;
            }
            else
            {
                if (Data != null && Data.Length > 0)
                {
                    WriteError(new ErrorRecord(new ArgumentException("Both pipeline and -Data arguments specified"), "", ErrorCategory.InvalidArgument, null));
                    return;
                }
                data = _inputObjects;
            }

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
