//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Volatility.CustomTypes;

//namespace Volatility.Primitives
//{
//    /// <summary>
//    /// Represents the arrow of time as a closed interval.
//    /// </summary>
//    public class Time<T>
//    {
//        #region
//        public static TimeBuilder builder;
//        #endregion

//        #region private fields
//        private double a;
//        private double b;
//        private double L;
//        private int nPoints;
//        private int nIntervals;
//        #endregion

//        #region private fields with implementation details
//        private int size;
//        protected IArray<T> data;

//        /// <summary>
//        /// Get the value at index i.
//        /// </summary>
//        /// <param name="i"></param>
//        /// <returns></returns>
//        public T this[int i]
//        {
//            get { return data.ElementAt(i, 0); }
//        }
//        #endregion

//        #region public properties
//        /// <summary>
//        /// Number of points inside the interval, extrema included.
//        /// </summary>
//        public int NmbPoints => nPoints;
//        /// <summary>
//        /// Number of sub-intervals into which the interval is divided.
//        /// </summary>
//        public int NmbIntervals => nIntervals;
//        /// <summary>
//        /// The left extrema of the time-interval.
//        /// </summary>
//        public double Start => a;
//        /// <summary>
//        /// The right extrema of the time-interval.
//        /// </summary>
//        public double End => b;
//        /// <summary>
//        /// The legth of the time interval.
//        /// </summary>
//        public double Length => L;
//        #endregion

//        /// <summary>
//        /// Inits the Time-arrow
//        /// </summary>
//        /// <param name="start">The left extreme of the time arrow. </param>
//        /// <param name="end">The right extreme of the time arrow. </param>
//        /// <param name="nPiece">The number of intervals into which the closed interval is divided</param>
//        public Time(IArray<T> data, double start, double end)
//        {

//            // compute the number of points
//            // compute the legth of the time interval "L"
//            // the function to compute the time at step i as t(i) = start + i * L / nPoints
//            // find the first power of 2 greater or equal than the number of points
//            // create an array<double> of the above size
//            // fill the array with the lineare function t(i)
//            this.a = start;
//            this.b = end;
//            this.L = end - start;
//            this.nIntervals = nIntervals;
//            this.nPoints = nIntervals + 1;
//            this.data = data;
//        }
//    }

//    /// <summary>
//    /// Handles the creation of the Time instance. 
//    /// </summary>
//    public class TimeBuilder<T>
//    {
//        /// <summary>
//        /// Returns time at point i
//        /// </summary>
//        /// <param name="i"></param>
//        /// <returns></returns>
//        private static T Time(T start, T end, int N, int i) => start + (end - start) * i / N;

//        static Dictionary<string, Func<T, T, int, int, T>> fillingMethods = new Dictionary<string, Func<T, T, int, int, T>>
//        {
//            ["linear"] = (a, b, N, i) => Time(a, b, N, i),
//        };

//        private static Func<int, double> GetFillerFunction(double start, double end, int nIntervals, string method)
//        {
//            switch (method)
//            {
//                case ("linear"):
//                    return (i) => fillingMethods[method](start, end, nIntervals, i);
//                default:
//                    throw new NotImplementedException($"{method} is not supported");
//            }
//        }

//        public static Time<T> Build<T>(T start, T end, int nIntervals = 2 ^ 10, string method = "linear")
//        {
//            CheckExtrema(start, end);
//            CheckPartitioning(nIntervals);

//            int size = FindSmallestPowerOfTwo(nIntervals);

//            Func<int, double> filler = GetFillerFunction(start, end, nIntervals, method);
//            double[] data = FillTimeArray(size, filler);
//        }

//        /// <summary>
//        /// Check that the start actually comes after the end
//        /// </summary>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        private static void CheckExtrema<T>(T start, T end) where T : IComparable<T>
//        {
//            if (start >= end)
//                throw new ArgumentException($"end must be greater than start argument");
//        }

//        /// <summary>
//        /// Check that the number of subintervals or the number of points required is strictly greater than zero
//        /// </summary>
//        /// <param name="N"></param>
//        private static void CheckPartitioning(int N)
//        {
//            if (N <= 0)
//                throw new ArgumentException($"partitioning argument must be greater than zero");
//        }

//        private static double[] FillTimeArray(int size, Func<int, double> t_i)
//        {
//            var obj = new double[size];
//            for (int i = 0; i < size; i++)
//                obj[i] = t_i(i);
//            return obj;
//        }
//    }

//    public class NumberFieldBuilder
//    {
//        protected static int FindSmallestPowerOfTwo(int N)
//        {
//            return (int)Math.Ceiling(Math.Log(N, 2.0));
//        }
//    }
//}
