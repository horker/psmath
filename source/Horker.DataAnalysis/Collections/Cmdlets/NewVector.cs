using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace Horker.DataAnalysis
{
    [Cmdlet("New", "Vector")]
    public class NewVector : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = false)]
        public object InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public int Size = 0;

        private Vector _vector;

        protected override void BeginProcessing()
        {
            _vector = new Vector();
        }

        protected override void ProcessRecord()
        {
            _vector.Add(InputObject);
        }

        protected override void EndProcessing()
        {
            if (Size != 0) {
                int dataCount = _vector.Count;
                if (dataCount > Size) {
                    _vector.RemoveRange(Size, dataCount - Size);
                }
                while (_vector.Count < Size) {
                    _vector.Add(_vector[_vector.Count % dataCount]);
                }
            }

            WriteObject(_vector);
        }
    }
}