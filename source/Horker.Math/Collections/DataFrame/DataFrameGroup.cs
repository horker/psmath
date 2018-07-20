using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horker.Math
{
    public class DataFrameGroup : OrderedDictionary
    {
        public new DataFrame this[object name] {
            get => (DataFrame)base[name];
            set => base[name] = (DataFrame)value;
        }

        public new DataFrame this[int index] {
            get => (DataFrame)base[index];
            set => base[index] = (DataFrame)value;
        }

        public double[][] ToDoubleJaggedArrayOf(string columnName)
        {
            var result = new double[this.Count][];

            for (var i = 0; i < result.Length; ++i) {
                result[i] = ((DataFrame)this[i])[columnName].ToDoubleArray();
            }

            return result;
        }
    }
}
