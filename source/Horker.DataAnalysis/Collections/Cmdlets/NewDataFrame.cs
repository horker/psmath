using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace Horker.DataAnalysis
{
    [Cmdlet("New", "DataFrame")]
    [CmdletBinding(DefaultParameterSetName = "Pipeline")]
    public class NewDataFrame : PSCmdlet
    {
        [Parameter(Position = 0, ValueFromPipeline = true, Mandatory = false, ParameterSetName = "Pipeline")]
        public PSObject InputObject;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "FromArray")]
        public object[] FromArray;

        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "FromArray")]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Diagonal")]
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Identity")]
        public int RowSize = int.MaxValue;

        [Parameter(Position = 2, Mandatory = false, ParameterSetName = "FromArray")]
        [Parameter(Position = 2, Mandatory = false, ParameterSetName = "Diagonal")]
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "Identity")]
        public int ColumnSize = int.MaxValue;

        [Parameter(Position = 3, Mandatory = false, ParameterSetName = "FromArray")]
        public SwitchParameter Transpose = false;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "FromJagged")]
        public object[][] FromJagged;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Diagonal")]
        public object Diagonal;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Identity")]
        public SwitchParameter Identity;

        private DataFrame _data;

        protected override void BeginProcessing()
        {
            _data = new DataFrame();
        }

        protected override void ProcessRecord()
        {
            if (InputObject != null) {
                _data.AddRow(InputObject);
            }
        }

        protected override void EndProcessing()
        {
            if (ParameterSetName == "FromJagged") {
                _data = DataFrame.Create(FromJagged);
            }
            else if (ParameterSetName == "FromArray") {
                _data = DataFrame.Create(FromArray, RowSize, ColumnSize, Transpose);
            }
            else if (ParameterSetName == "Diagonal") {
                _data = DataFrame.Diagonal(Diagonal, RowSize, ColumnSize);
            }
            else if (ParameterSetName == "Identity") {
                _data = DataFrame.Identity(RowSize, ColumnSize);
            }

            WriteObject(_data.LinkedPSObject);
        }
    }
}
