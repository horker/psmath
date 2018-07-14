using System;
using System.Linq;
using System.Management.Automation;

namespace Horker.DataAnalysis
{
    public class KalmanFilter2DResult
    {
        public double X;
        public double Y;
        public double XAxisVelocity;
        public double YAxisVelocity;
        public double NoiseX;
        public double NoiseY;
    }

    [Cmdlet("Get", "Math.KalmanFilter2D")]
    [Alias("math.kalman")]
    public class KalmanFilter2D : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object[] X;

        [Parameter(Position = 1, Mandatory = true)]
        public object[] Y;

        [Parameter(Position = 2, Mandatory = false)]
        public double SamplingRate = 1.0;

        [Parameter(Position = 3, Mandatory = false)]
        public double Acceleration = 0.0005;

        [Parameter(Position = 4, Mandatory = false)]
        public double AccelerationStdDev = 0.1;

        protected override void EndProcessing()
        {
            var xSeries = X.Select(x => Converter.ToDouble(x)).ToArray();
            var ySeries = Y.Select(x => Converter.ToDouble(x)).ToArray();

            if (xSeries.Length != ySeries.Length)
            {
                WriteError(new ErrorRecord(new ArgumentException("X and Y should have the same length"), "", ErrorCategory.InvalidArgument, null));
                return;
            }

            var filter = new Accord.Statistics.Running.KalmanFilter2D(SamplingRate, Acceleration, AccelerationStdDev);

            for (var i = 0; i < xSeries.Length; ++i)
            {
                filter.Push(xSeries[i], ySeries[i]);

                var result = new KalmanFilter2DResult();
                result.X = filter.X;
                result.Y = filter.Y;
                result.XAxisVelocity = filter.XAxisVelocity;
                result.YAxisVelocity = filter.YAxisVelocity;
                result.NoiseX = filter.NoiseX;
                result.NoiseY = filter.NoiseY;

                WriteObject(result);
            }
        }
    }
}
