﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Math.Random;

namespace Horker.Math.PSObjects
{
    [Cmdlet("Get", "PSObject.Sample")]
    [Alias("pso.sample")]
    public class GetPSObjectSample : ObjectListCmdletBase<object>
    {
        [Parameter(Position = 1, Mandatory = true)]
        public int Size;

        [Parameter(Position = 2, Mandatory = false)]
        public SwitchParameter Replacement;

        protected override void Process(IReadOnlyList<object> values)
        {
            if (Replacement)
            {
                for (var i = 0; i < Size; ++i)
                {
                    var j = Generator.Random.Next(values.Count);
                    WriteObject(values[j]);
                }
            }
            else
            {
                var samples = Vector.Sample(Size, values.Count);
                for (var i = 0; i < Size; ++i)
                    WriteObject(values[samples[i]]);
            }
        }
    }

    [Cmdlet("Get", "PSObject.Shuffle")]
    [Alias("pso.shuffle")]
    public class GetPSObjectShuffle : ObjectListCmdletBase<object>
    {
        protected override void Process(IReadOnlyList<object> values)
        {
            // ref. https://en.wikipedia.org/wiki/Fisher-Yates_shuffle

            int count = values.Count;

            var table = new object[count];
            for (var i = 0; i < count; ++i) {
                var j = Generator.Random.Next(i + 1);
                if (j != i) {
                    table[i] = table[j];
                }
                table[j] = values[i];
            }

            for (var i = 0; i < count; ++i) {
                WriteObject(table[i]);
            }
        }
    }

    [Cmdlet("Get", "PSObject.Split")]
    [Alias("pso.split")]
    public class NewPSObjectSplit : ObjectListCmdletBase<object>
    {
        [Parameter(Position = 1, Mandatory = true)]
        public double[] Rates;

        protected override void Process(IReadOnlyList<object> values)
        {
            var results = ArrayMethods.AdditionalMethods.SplitInternal(values, Rates);

            foreach (var r in results)
                WriteObject(r);
        }
    }
}