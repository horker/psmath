using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Management.Automation;
using Accord.Statistics.Analysis;
using Accord.Statistics.Filters;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;
using Accord.Statistics.Links;
using Accord.Math;

namespace Horker.Math
{
    public enum LinkFunctionType
    {
        Absolute,
        Cauchit,
        Identity,
        Inverse,
        InverseSquared,
        Logit,
        Log,
        LogLog,
        Probit,
        Sin,
        Threshold
    }

    public class LinkFunctionConvert
    {
        public static ILinkFunction Get(LinkFunctionType link)
        {
            ILinkFunction result = null;
            switch (link) {
                case LinkFunctionType.Absolute: result = new AbsoluteLinkFunction(); break;
                case LinkFunctionType.Cauchit: result = new CauchitLinkFunction(); break;
                case LinkFunctionType.Identity: result = new IdentityLinkFunction(); break;
                case LinkFunctionType.Inverse: result = new InverseLinkFunction(); break;
                case LinkFunctionType.InverseSquared: result = new InverseSquaredLinkFunction(); break;
                case LinkFunctionType.Logit: result = new LogitLinkFunction(); break;
                case LinkFunctionType.Log: result = new LogLinkFunction(); break;
                case LinkFunctionType.LogLog: result = new LogLogLinkFunction(); break;
                case LinkFunctionType.Probit: result = new ProbitLinkFunction(); break;
                case LinkFunctionType.Sin: result = new SinLinkFunction(); break;
                case LinkFunctionType.Threshold: result = new ThresholdLinkFunction(); break;
            }

            return result;
        }
    }

    [Cmdlet("Invoke", "GeneralizedLinearRegression")]
    [CmdletBinding(DefaultParameterSetName = "Pipeline")]
    public class InvokeGeneralizedLinearRegression : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "XY")]
        public object X;

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "XY")]
        public object Y;

        [Parameter(ValueFromPipeline = true, Mandatory = true, ParameterSetName = "Pipeline")]
        public PSObject InputObject;

        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Pipeline")]
        public string OutputName;

        [Parameter(Mandatory = true)]
        public LinkFunctionType LinkFunction;

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

            if (ParameterSetName == "XY") {
                inputs = Converter.ToDoubleJaggedArray(X);
                outputs = Converter.ToDoubleArray(Y);
            }
            else {
                outputs = _data.GetColumn(OutputName).ToDoubleArray();

                _data.RemoveColumn(OutputName);
                inputs = _data.ToDoubleJaggedArray();
            }

            double[] w = null;
            if (Weights != null) {
                w = Converter.ToDoubleArray(Weights);
            }

            var model = new GeneralizedLinearRegression(LinkFunctionConvert.Get(LinkFunction)) {
                NumberOfInputs = inputs[0].Length
            };

            var learner = new IterativeReweightedLeastSquares(model) {
                MaxIterations = 200,
                Regularization = 0
            };

            learner.Learn(inputs, outputs, w);

            WriteObject(model);
        }
    }
}