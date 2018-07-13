using System.Collections.Generic;
using System.Management.Automation;
using Accord.Statistics.Distributions;
using Accord.Statistics.Distributions.Univariate;

namespace Horker.DataAnalysis
{
    class DistributionHelper
    {
        private static string[][] Aliases = {
            // Cumultive distribution function
            new string[] {"Cdf",    "$this.DistributionFunction($args[0])" },

            // Survival function
            new string[] {"Ccdf",   "$this.ComplementaryDistributionFunction($args[0])" },

            // Quantile function
            new string[] {"Icdf",   "$this.InverseDistributionFunction($args[0])" },

            // Quantile dentisy function
            new string[] {"Qdf",    "$this.QuantileDensityFunction($args[0])" },

            // Hazard functions
            new string[] {"Hf",     "$this.HazardFunction($args[0])" },
            new string[] {"Chf",    "$this.CumulativeHazardFunction($args[0])" },
            new string[] {"LogChf", "$this.LogCumulativeHazardFunction($args[0])" }
        };

        private static string[][] DiscreteAliases = {
            // Probability mass functions
            new string[] {"Pmf",    "$this.ProbabilityMassFunction($args[0])" },
            new string[] {"LogPmf", "$this.LogProbabilityMassFunction($args[0])" },
        };

        private static string[][] ContinuousAliases = {
            // Probability density functions
            new string[] {"Pdf",    "$this.ProbabilityDensityFunction($args[0])" },
            new string[] {"LogPdf", "$this.LogProbabilityDensityFunction($args[0])" },
        };

        public static PSObject AddConvinienceMethods(IUnivariateDistribution dist)
        {
            var obj = new PSObject(dist);

            foreach (var a in Aliases)
            {
                obj.Methods.Add(new PSScriptMethod(a[0], ScriptBlock.Create(a[1])));
            }

            if (dist is UnivariateDiscreteDistribution)
            {
                foreach (var a in DiscreteAliases)
                {
                    obj.Methods.Add(new PSScriptMethod(a[0], ScriptBlock.Create(a[1])));
                }
            }
            else
            {
                foreach (var a in ContinuousAliases)
                {
                    obj.Methods.Add(new PSScriptMethod(a[0], ScriptBlock.Create(a[1])));
                }
            }

            return obj;
        }
    }

    [Cmdlet("New", "BernoulliDistribution")]
    [Alias("st.bernoulli")]
    public class NewBernoulliDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public double Mean = .5;

        protected override void EndProcessing()
        {
            var dist = new BernoulliDistribution(Mean);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "BetaDistribution")]
    [Alias("st.beta")]
    [CmdletBinding(DefaultParameterSetName = "Alpha")]
    public class NewBetaDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Alpha")]
        public double Alpha;

        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Alpha")]
        public double Beta;

        [Parameter(Mandatory = true, ParameterSetName = "Trials")]
        public int Successes;

        [Parameter(Mandatory = true, ParameterSetName = "Trials")]
        public int Trials;

        protected override void EndProcessing()
        {
            BetaDistribution dist;

            if (ParameterSetName == "Alpha")
            {
                dist = new BetaDistribution(Alpha, Beta);
            }
            else
            {
                dist = new BetaDistribution(Successes, Trials);
            }
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "BinomialDistribution")]
    [Alias("st.binomial")]
    public class NewBinomialDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int Trials;

        [Parameter(Position = 1, Mandatory = false)]
        public double Probability = .5;

        protected override void EndProcessing()
        {
            var dist = new BinomialDistribution(Trials, Probability);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "CauchyDistribution")]
    [Alias("st.cauchy")]
    public class NewCauchyDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public double Location = 0.0;

        [Parameter(Position = 1, Mandatory = false)]
        public double Scale = 1.0;

        protected override void EndProcessing()
        {
            var dist = new CauchyDistribution(Location, Scale);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "ChiSquareDistribution")]
    [Alias("st.chisquire")]
    public class NewChiSquareDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int DegreesOfFreedom;

        protected override void EndProcessing()
        {
            var dist = new ChiSquareDistribution(DegreesOfFreedom);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "EmpiricalDistribution")]
    [Alias("st.empirical")]
    public class NewEmpiricalDistribution : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true)]
        public double InputObject;

        [Parameter(Position = 0, Mandatory = false)]
        public double Smoothing = double.NaN;

        private List<double> _data;

        protected override void BeginProcessing()
        {
            _data = new List<double>();
        }

        protected override void ProcessRecord()
        {
            _data.Add(Converter.ToDouble(InputObject));
        }

        protected override void EndProcessing()
        {
            EmpiricalDistribution dist;
            if (double.IsNaN(Smoothing))
            {
                dist = new EmpiricalDistribution(_data.ToArray());
            }
            else
            {
                dist = new EmpiricalDistribution(_data.ToArray(), Smoothing);
            }

            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "ExponentialDistribution")]
    [Alias("st.exponential")]
    public class NewExponentialDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public double Rate = 1.0;

        protected override void EndProcessing()
        {
            var dist = new ExponentialDistribution(Rate);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "FDistribution")]
    [Alias("st.f")]
    public class NewFDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int Degrees1;

        [Parameter(Position = 1, Mandatory = true)]
        public int Degrees2;

        protected override void EndProcessing()
        {
            var dist = new FDistribution(Degrees1, Degrees2);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "GammaDistribution")]
    [Alias("st.gamma")]
    public class NewGammaDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double Theta = 1.0;

        [Parameter(Position = 1, Mandatory = true)]
        public double K = 1.0;

        protected override void EndProcessing()
        {
            var dist = new GammaDistribution(Theta, K);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "GeometricDistribution")]
    [Alias("st.geometric")]
    public class NewGeometricDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double ProbabilityOfSuccess;

        protected override void EndProcessing()
        {
            var dist = new GeometricDistribution(ProbabilityOfSuccess);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "HypergeometricDistribution")]
    [Alias("st.hypergeo")]
    public class NewHypergeometricDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int PopulationSize;

        [Parameter(Position = 1, Mandatory = true)]
        public int Successes;

        [Parameter(Position = 2, Mandatory = true)]
        public int Samples;

        protected override void EndProcessing()
        {
            var dist = new HypergeometricDistribution(PopulationSize, Successes, Samples);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "LogisticDistribution")]
    [Alias("st.logistic")]
    public class NewLogisticDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public double Location = 0.0;

        [Parameter(Position = 1, Mandatory = false)]
        public double Scale = 1.0;

        protected override void EndProcessing()
        {
            var dist = new LogisticDistribution(Location, Scale);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "LognormalDistribution")]
    [Alias("st.lognormal")]
    public class NewLognormalDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public double MeanLog = 0.0;

        [Parameter(Position = 1, Mandatory = false)]
        public double ScaleLog = 1.0;

        protected override void EndProcessing()
        {
            var dist = new LognormalDistribution(MeanLog, ScaleLog);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "NegativeBinomialDistribution")]
    [Alias("st.negbinomial")]
    public class NewNegativeBinomialDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public int Failures = 1;

        [Parameter(Position = 1, Mandatory = false)]
        public double Probability = .5;

        protected override void EndProcessing()
        {
            var dist = new NegativeBinomialDistribution(Failures, Probability);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "NormalDistribution")]
    [Alias("st.normal")]
    public class NewNormalDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public double Mean = 0.0;

        [Parameter(Position = 1, Mandatory = false)]
        [Alias("StdDev")]
        public double StandardDeviation = 1.0;

        protected override void EndProcessing()
        {
            var dist = new NormalDistribution(Mean, StandardDeviation);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "PoisonDistribution")]
    [Alias("st.poison")]
    public class NewPoisonDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public double Lambda = 1.0;

        protected override void EndProcessing()
        {
            var dist = new PoissonDistribution(Lambda);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "TDistribution")]
    [Alias("st.t")]
    public class NewTDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public int DegreesOfFreedom = 1;

        protected override void EndProcessing()
        {
            var dist = new TDistribution(DegreesOfFreedom);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "UniformContinuousDistribution")]
    [Alias("st.uniform")]
    public class NewUniformContinuousDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public double Minimum = 0.0;

        [Parameter(Position = 1, Mandatory = false)]
        public double Maximum = 1.0;

        protected override void EndProcessing()
        {
            var dist = new UniformContinuousDistribution(Minimum, Maximum);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "UniformDiscreteDistribution")]
    [Alias("st.uniformd")]
    public class NewUniformDiscreteDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public int Minimum = 0;

        [Parameter(Position = 1, Mandatory = false)]
        public int Maximum = 1;

        protected override void EndProcessing()
        {
            var dist = new UniformDiscreteDistribution(Minimum, Maximum);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }

    [Cmdlet("New", "WeibullDistribution")]
    [Alias("st.weibull")]
    public class NewWeibullDistribution : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double Shape;

        [Parameter(Position = 1, Mandatory = false)]
        public double Scale = 1.0;

        protected override void EndProcessing()
        {
            var dist = new WeibullDistribution(Shape, Scale);
            var obj = DistributionHelper.AddConvinienceMethods(dist);
            WriteObject(obj);
        }
    }
}