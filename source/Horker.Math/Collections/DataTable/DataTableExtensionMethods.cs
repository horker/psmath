using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horker.Math
{
    public static class DataTableExtensionMethods
    {
        public static void ExpandToOneHot(this DataTable table, string columnName, int total = -1, bool preserve = false, string columnNameTemplate = "{0}_{1}")
        {
            var index = table.Columns.IndexOf(columnName);
            var dataType = table.Columns[columnName].DataType;

            if (total == -1)
            {
                foreach (DataRow row in table.Rows)
                {
                    var value = Convert.ToInt32(row[index]);
                    if (total < value)
                        total = value;
                }
                ++total;
            }

            for (var i = 0; i < total; ++i)
            {
                var column = new DataColumn(string.Format(columnNameTemplate, columnName, i));
                column.DataType = dataType;
                table.Columns.Add(column);
                column.SetOrdinal(index + 1 + i);
            }

            foreach (DataRow row in table.Rows)
            {
                for (var i = 0; i < total; ++i)
                    row[index + 1 + i] = i == Convert.ToInt32(row[index]) ? 1 : 0;
            }

            if (!preserve)
                table.Columns.RemoveAt(index);
        }
    }
}
