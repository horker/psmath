using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Diagnostics;
using System.Data;

namespace Horker.Math
{
    [Cmdlet("New", "Math.DataFrame")]
    [Alias("math.df")]
    [CmdletBinding(DefaultParameterSetName = "psobjects")]
    [OutputType(typeof(DataFrame))]
    public class NewMathDataFrame : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = true, ParameterSetName = "psobjects")]
        public PSObject InputObject;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "datatable")]
        public DataTable DataTable;

        private DataFrame _df;

        protected override void BeginProcessing()
        {
            if (ParameterSetName == "psobjects")
                _df = new DataFrame();
        }

        protected override void ProcessRecord()
        {
            if (InputObject != null)
                _df.AddRow(InputObject);
        }

        protected override void EndProcessing()
        {
            if (ParameterSetName == "datatable")
                _df = DataFrame.FromDataTable(DataTable);

            var link = _df.LinkedPSObject;
            WriteObject(link);
        }
    }
}
