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
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "WithValue")]
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Zero")]
        public int RowSize = int.MaxValue;

        [Parameter(Position = 2, Mandatory = false, ParameterSetName = "FromArray")]
        [Parameter(Position = 2, Mandatory = false, ParameterSetName = "Diagonal")]
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "Identity")]
        [Parameter(Position = 2, Mandatory = false, ParameterSetName = "WithValue")]
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "Zero")]
        public int ColumnSize = int.MaxValue;

        [Parameter(Position = 3, Mandatory = false, ParameterSetName = "FromArray")]
        public SwitchParameter Transpose = false;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "FromJagged")]
        public object[][] FromJagged;

        [Parameter(Mandatory = true, ParameterSetName = "Diagonal")]
        public SwitchParameter Diagonal;

        [Parameter(Position = 0, Mandatory = false, ParameterSetName = "Diagonal")]
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "WithValue")]
        public object Value = 1.0;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Identity")]
        public SwitchParameter Identity;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "WithValue")]
        public SwitchParameter WithValue;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Zero")]
        public SwitchParameter Zero;

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
                _data = new DataFrame(FromJagged);
            }
            else if (ParameterSetName == "FromArray") {
                _data = new DataFrame(FromArray, RowSize, ColumnSize, Transpose);
            }
            else if (ParameterSetName == "Diagonal") {
                _data = DataFrame.Diagonal(Value, RowSize, ColumnSize);
            }
            else if (ParameterSetName == "WithValue") {
                _data = DataFrame.WithValue(Value, RowSize, ColumnSize);
            }
            else if (ParameterSetName == "Identity") {
                _data = DataFrame.Identity(RowSize, ColumnSize);
            }
            else if (ParameterSetName == "Zero") {
                _data = DataFrame.Zero(RowSize, ColumnSize);
            }

            WriteObject(_data.LinkedPSObject);
        }
    }
}
