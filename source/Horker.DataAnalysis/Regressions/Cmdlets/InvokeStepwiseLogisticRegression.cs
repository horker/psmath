using System.Linq;
using System.Data;
using System.Management.Automation;
using Accord.Statistics.Analysis;
using Accord.Math;

namespace Horker.DataAnalysis
{
    [Cmdlet("Invoke", "StepwiseLogisticRegression")]
    [CmdletBinding(DefaultParameterSetName = "Pipeline")]
    public class InvokeStepwiseLogisticRegression : PSCmdlet
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
            double[][] inputs;
            double[] outputs;
            string[] inputNames;
            string outputName;

            if (ParameterSetName == "XY") {
                inputs = Converter.ToDoubleJaggedArray(X);
                outputs = Converter.ToDoubleArray(Y);

                inputNames = Accord.Math.Vector.Range(0, inputs[0].Length, 1).Select(x => "x" + x).ToArray();
                outputName = "y";
            }
            else {
                outputs = _data.GetColumn(OutputName).ToDoubleArray();

                _data.RemoveColumn(OutputName);
                inputs = _data.ToDoubleJaggedArray();

                inputNames = _data.ColumnNames.ToArray();
                outputName = OutputName;
            }

            double[] w = null;
            if (Weights != null) {
                w = Converter.ToDoubleArray(Weights);
            }

            var model = new StepwiseLogisticRegressionAnalysis(inputs, outputs, inputNames, outputName);

#pragma warning disable CS0618
            model.Compute();
#pragma warning restore CS0618

            WriteObject(model);
        }
    }
}