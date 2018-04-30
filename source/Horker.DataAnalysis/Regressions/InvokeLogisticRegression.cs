using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Management.Automation;
using Accord.Statistics.Analysis;
using Accord.Statistics.Filters;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;
using Accord.Math;

namespace Horker.DataAnalysis
{
    [Cmdlet("Invoke", "LogisticRegression")]
    [CmdletBinding(DefaultParameterSetName = "Pipeline")]
    public class InvokeLogisticRegression : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "XY")]
        public object X;

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "XY")]
        public object Y;

        [Parameter(ValueFromPipeline = true, Mandatory = true, ParameterSetName = "Pipeline")]
        public PSObject InputObject;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Pipeline")]
        public string OutputName;

        [Parameter(Mandatory = false)]
        public object Weights;

        private DataFrame _data;

        protected override void BeginProcessing()
        {
            _data = new DataFrame();
        }

        protected override void ProcessRecord()
        {
            if (ParameterSetName != "Pipeline") {
                return;
            }

            _data.AddRow(InputObject);
        }

        protected override void EndProcessing()
        {
            var model = new LogisticRegressionAnalysis();

            double[][] inputs;
            double[] outputs;

            if (ParameterSetName == "XY") {
                inputs = Converter.ToDoubleJaggedArray(X);
                outputs = Converter.ToDoubleArray(Y);
            }
            else {
                outputs = _data.GetColumn(OutputName).ToDoubleArray();

                _data.RemoveColumn(OutputName);
                inputs = _data.ToDoubleJaggedArray();

                model.Inputs = _data.ColumnNames.ToArray<string>();
                model.Output = OutputName;
            }

            double[] w = null;
            if (Weights != null) {
                w = Converter.ToDoubleArray(Weights);
            }

            model.Learn(inputs, outputs, w);

            WriteObject(model);
        }
    }
}