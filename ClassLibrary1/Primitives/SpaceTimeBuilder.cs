//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Volatility.Primitives
//{
//    public class StateSpace
//    {
//        private Time time;
//        private Space space;

//        public double T(int i) => time[i];

//        public StateSpace(Time t, Space s)
//        {
//            time = t;
//            space = s;
//        }
//    }


//    /// <summary>
//    /// Builds the state space of the problem, e.g. [0, T] x R 
//    /// for a single stock traded in a finite time interval
//    /// </summary>
//    public class StateSpaceBuilder: NumberFieldBuilder
//    {
//        #region static methods

//        #endregion

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <param name="nIntervals"></param>
//        /// <param name="spaceDim">Dimension of the State space (not counting time)</param>
//        /// <returns></returns>
//        public static StateSpace Build(double start, double end, int nIntervals = 2 ^ 10, int spaceDim = 1)
//        {
//            // the number of points is the number of subintervals + 1 and the number of dimensions must account also for time 
//            double[][] sharedMemory = InitSharedMemory(nIntervals + 1, spaceDim + 1);

//            Time time = new Time(sharedMemory[0], start, end, nIntervals);
//            Space space = new Space();
//            return new StateSpace(time, space);
//        }

//        /// <summary>
//        /// Creates a 2D array [[t_1, x_11, x_21, x_31], [t_2, x_21, x_22, z_32], ..]
//        /// with empty values.
//        /// </summary>
//        /// <remarks>The layout is optimized to access the state at time t. I should try jagged arrays or trasposing. </remarks>
//        /// <returns></returns>
//        public static double[][] InitSharedMemory(int nmbElements, int nmbDimensions)
//        {
//            int size = FindSmallestPowerOfTwo(nmbElements);
//            double[][] arrayDims = new double[nmbDimensions][];
//            for (int i = 0; i < nmbDimensions; i++)
//                arrayDims[i] = new double[size];
//            return arrayDims;
//        }
//    }

//}
