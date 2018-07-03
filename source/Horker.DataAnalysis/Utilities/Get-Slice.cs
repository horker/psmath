using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace Horker.DataAnalysis
{
    public class Slice<T> : IEnumerable<T>
    {
        private ArraySegment<T> _segment;

        public Slice(T[] array, int offset, int count)
        {
            _segment = new ArraySegment<T>(array, offset, count);
        }

        public T[] Array { get { return _segment.Array;  } }
        public int Offset { get { return _segment.Offset; } }
        public int Count { get { return _segment.Count; } }

        public T this[int index]
        {
            get {
                if (index < 0 || index >= _segment.Count) {
                    throw new ArgumentOutOfRangeException("index out of range");
                }
                return _segment.Array[index + _segment.Offset];
            }

            set {
                if (index < 0 || index >= _segment.Count) {
                    throw new ArgumentOutOfRangeException("index out of range");
                }
                _segment.Array[index + _segment.Offset] = value;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_segment).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)_segment).GetEnumerator();
        }
    }

    [Cmdlet("Get", "Slice")]
    [Alias("slice")]
    public class GetSpan : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object[] Array;

        [Parameter(Position = 1, Mandatory = true)]
        public int Offset;

        [Parameter(Position = 2, Mandatory = true)]
        public int Count;

        protected override void EndProcessing()
        {
            WriteObject(new Slice<object>(Array, Offset, Count));
        }
    }
}
