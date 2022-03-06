using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volatility.CustomTypes;
using Volatility.Utils;

namespace Volatility.Primitives
{
    /// <summary>
    /// Rerpresents the state space 
    /// </summary>
    public class Space<T>
    {
        #region private fields
        private int nPoints;
        private int nIntervals;
        #endregion

        #region private fields with implementation details
        private int size;
        private IArray<T> space;

        /// <summary>
        /// Get time t_i at index i.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T this[int i, int j]
        {
            get { return space[i, j]; }
        }
        #endregion

        #region public properties
        /// <summary>
        /// Number of points inside the interval, extrema included.
        /// </summary>
        public int NmbPoints => nPoints;
        /// <summary>
        /// Number of sub-intervals into which the interval is divided.
        /// </summary>
        public int NmbIntervals => nIntervals;
        #endregion

    }
}
