using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Testing;

namespace Horker.Math
{
    public class TestingHelper
    {
        public static OneSampleHypothesis GetOneSampleHypothesis(string alternate)
        {
            OneSampleHypothesis hypo;
            switch (alternate) {
                case "Different":
                    hypo = OneSampleHypothesis.ValueIsDifferentFromHypothesis;
                    break;

                case "Greater":
                    hypo = OneSampleHypothesis.ValueIsGreaterThanHypothesis;
                    break;

                case "Smaller":
                    hypo = OneSampleHypothesis.ValueIsSmallerThanHypothesis;
                    break;

                default:
                    throw new RuntimeException("Invalid Alternate value");
            }

            return hypo;
        }

        public static TwoSampleHypothesis GetTwoSampleHypothesis(string alternate)
        {
            TwoSampleHypothesis hypo;
            switch (alternate) {
                case "Different":
                    hypo = TwoSampleHypothesis.ValuesAreDifferent;
                    break;

                case "Greater":
                    hypo = TwoSampleHypothesis.FirstValueIsGreaterThanSecond;
                    break;

                case "Smaller":
                    hypo = TwoSampleHypothesis.FirstValueIsSmallerThanSecond;
                    break;

                default:
                    throw new RuntimeException("Invalid Alternate value");
            }

            return hypo;
        }

        public static KolmogorovSmirnovTestHypothesis GetKolmogorovSmirnovTestHypothesis(string alternate)
        {
            KolmogorovSmirnovTestHypothesis hypo;
            switch (alternate) {
                case "Different":
                    hypo = KolmogorovSmirnovTestHypothesis.SampleIsDifferent;
                    break;

                case "Greater":
                    hypo = KolmogorovSmirnovTestHypothesis.SampleIsGreater;
                    break;

                case "Smaller":
                    hypo = KolmogorovSmirnovTestHypothesis.SampleIsSmaller;
                    break;

                default:
                    throw new RuntimeException("Invalid Alternate value");
            }

            return hypo;
        }

        public static TwoSampleKolmogorovSmirnovTestHypothesis GetTwoSampleKolmogorovSmirnovTestHypothesis(string alternate)
        {
            TwoSampleKolmogorovSmirnovTestHypothesis hypo;
            switch (alternate) {
                case "Different":
                    hypo = TwoSampleKolmogorovSmirnovTestHypothesis.SamplesDistributionsAreUnequal;
                    break;

                case "Greater":
                    hypo = TwoSampleKolmogorovSmirnovTestHypothesis.FirstSampleIsLargerThanSecond;
                    break;

                case "Smaller":
                    hypo = TwoSampleKolmogorovSmirnovTestHypothesis.FirstSampleIsSmallerThanSecond;
                    break;

                default:
                    throw new RuntimeException("Invalid Alternate value");
            }

            return hypo;
        }
    }
}
