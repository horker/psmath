using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Horker.Math
{
    public class DataTablePSMethods
    {
        public static IEnumerable<object> GetColumn(PSObject value, string columnName)
        {
            var table = value.BaseObject as DataTable;
            return table.GetColumn(columnName);
        }

        public static object ExpandToOneHot(PSObject value, string columnName, int total = -1, bool preserve = false, string columnNameTemplate = "{0}_{1}")
        {
            var table = value.BaseObject as DataTable;
            table.ExpandToOneHot(columnName, total, preserve, columnNameTemplate);

            return null;
        }
    }
}
