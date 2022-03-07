using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volatility.CustomTypes
{
    /// Array.Copy seems to be faster than Buffer.BlockCopy
    ///  - https://stackoverflow.com/questions/1389821/array-copy-vs-buffer-blockcopy
    /// Using Enumerable to get a copy of a column/row of an array:
    ///  - https://stackoverflow.com/questions/27427527/how-to-get-a-complete-row-or-column-from-2d-array-in-c-sharp
    /// C# is row- major: 
    ///  - https://www.google.com/search?client=firefox-b-d&q=is+C%23+row+major+or+column+major
    /// Row major memory layout
    ///  - https://www.google.com/search?q=%22c%23%22+multidimensional+array+memory+layout&tbm=isch&ved=2ahUKEwjZ67zy0K72AhVIvCoKHR3SDi4Q2-cCegQIABAA&oq=%22c%23%22+multidimensional+array+memory+layout&gs_lcp=CgNpbWcQAzIECAAQGFC1BVikDWCQE2gAcAB4AIABgwGIAcACkgEDMi4xmAEAoAEBqgELZ3dzLXdpei1pbWfAAQE&sclient=img&ei=6ikjYtnOAcj4qgGdpLvwAg&bih=711&biw=1536&client=firefox-b-d#imgrc=gpcPpawBMP0QzM
    ///  - iterate over a row, then you will find yourself immediately at the beginning of the successive row
    /// On IEnumerable<T>
    ///  - https://stackoverflow.com/questions/8760322/troubles-implementing-ienumerablet

    public class MultidimensionalArray<T> : IArray<T>
    {
        /// <summary>
        /// Rectangular boundaries
        /// </summary>
        public int MinRow { get; private set; } = 0;
        public int MaxRow { get; private set; }
        public int MinCol { get; private set; } = 0;
        public int MaxCol { get; private set; }
        public int NmbRows { get; private set; }
        public int NmbCols { get; private set; }

        private T[,] state;

        public MultidimensionalArray(int nmbRows, int nmbCols)
        {
            state = new T[nmbRows, nmbCols];
            NmbRows = nmbRows;
            NmbCols = nmbCols;
            MaxRow = nmbRows - 1;
            MaxCol = nmbCols - 1;
        }

        public T Get(int rowI, int colJ)
        {
            // with safe access
            if (rowI < MinRow || rowI > MaxRow || colJ < MinCol || colJ > MaxCol)
                throw new ArgumentOutOfRangeException($"Ensure for cols {MinRow}<={rowI}<={MaxRow} and for rows {MinCol}<=i:{colJ}<={MaxCol}");
            return state[rowI, colJ];
        }

        public void Set(int rowI, int colJ, T value)
        {
            // with safe access
            if (rowI < MinRow || rowI > MaxRow || colJ < MinCol || colJ > MaxCol)
                throw new ArgumentOutOfRangeException($"Ensure for cols {MinRow}<={rowI}<={MaxRow} and for rows {MinCol}<=i:{colJ}<={MaxCol}");
            state[rowI, colJ] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowI"></param>
        /// <param name="colJ"></param>
        /// <returns></returns>
        public T this[int rowI, int colJ]
        {
            get
            {
                return state[rowI, colJ];
            }
            set
            {
                state[rowI, colJ] = value;
            }
        }


        /// <summary>
        /// IEnumerators in C#
        ///  - https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable.getenumerator?view=net-6.0
        /// lmao
        ///   - https://forum.unity.com/threads/how-to-release-array-memory.478325/
        /// </summary>
        private IEnumerable<T> Col(int idx, int startRow, int endRow)
        {
            for (int i = startRow; i < endRow + 1; i++)
                yield return state[i, idx];
        }

        public IEnumerable<T> SelectCol(int idx)
        {
            return Col(idx, MinRow, MaxRow);
        }

        /// <summary>
        /// Projection?
        ///  - https://stackoverflow.com/questions/1160217/changing-element-value-in-listt-foreach-foreach-method
        /// Safe & efficient code
        ///  - https://docs.microsoft.com/en-us/dotnet/csharp/write-safe-efficient-code
        /// </summary>
        private void Apply(int idx, int start, int end, Func<T, T> projection)
        {
            for (int i = start; i < end + 1; i++)
            {
                state[i, idx] = projection(state[i, idx]);
            }
        }

        public void Apply(int idx, Func<T, T> projection)
        {
            Apply(idx, MinRow, MaxRow, projection);
        }
    }
}
