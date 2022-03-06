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

        public ISlice<T> Slice()
        {
            return new UnidimensionalSlice<T>(this);
        }
    }

    /// <summary>
    /// Projection?
    ///  - https://stackoverflow.com/questions/1160217/changing-element-value-in-listt-foreach-foreach-method
    /// </summary>
    public static class Extension
    {
        public static IEnumerable<T> Col<T>(this MultidimensionalArray<T> arr, int idx)
        {
            return arr.Slice().SelectCol(idx);
        }

        /// <summary>
        /// Sets elements on the whole column. How to allow arbitrary starting point?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="idx"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public static IEnumerable<T> Col<T>(this MultidimensionalArray<T> arr, int idx, Func<T, T> projection)
        {
            return arr.Slice().SelectCol(idx, projection);
        }

        public static void ApplyChanges<T>(this MultidimensionalArray<T> arr, IEnumerable<T> instructions)
        {

        }
    }


    /// <summary>
    /// Safe & efficient code
    ///  - https://docs.microsoft.com/en-us/dotnet/csharp/write-safe-efficient-code
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISlice<T>: IEnumerator<T>
    {
        IEnumerable<T> SelectCol(int idx);
        IEnumerable<T> SelectCol(int idx, Func<T, T> projection);
        void ApplyChanges(IEnumerable<T> instructions);
    }

    /// <summary>
    /// Enumerators in C#
    ///  - https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable.getenumerator?view=net-6.0
    /// lmao
    ///   - https://forum.unity.com/threads/how-to-release-array-memory.478325/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UnidimensionalSlice<T>: ISlice<T>, IEnumerator<T>, IEnumerable<T>
    {
        private MultidimensionalArray<T> state;
        private int fixedIndex;
        private int start;
        private int end;
        private int position;
        private bool isCol;

        public T Current => isCol? state[position, fixedIndex]: state[fixedIndex, position];

        object System.Collections.IEnumerator.Current => Current;

        // Implementation for the GetEnumerator method.
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = start; i < end; i++)
                yield return Current;
        }

        private void Set(int idx, int start, int end, bool isCol)
        {
            this.fixedIndex = idx;
            this.start = start;
            this.end = end;
            this.position = start;
            this.isCol = isCol;
        }

        public void ApplyChanges(IEnumerable<T> instructions)
        {
            var it = instructions.GetEnumerator();
            _ = it.MoveNext();
            for (int i = start; i < end + 1; i++)
            {
                for (int j = start; i < end + 1; i++)
                    state[i, j] = it.Current;
                _ = it.MoveNext();
            }
        }

        private IEnumerable<T> SelectCol(int idx, int start, int end, Func<T, T> projection)
        {
            Set(idx, start, end, true);

            for (int i = start; i < end + 1; i++)
            {
                yield return projection(state[i, fixedIndex]);
            }
        }
        public IEnumerable<T> SelectCol(int idx, Func<T, T> projection)
        {
            return SelectCol(idx, state.MinRow, state.MaxRow, projection);
        }

        private IEnumerable<T> SelectCol(int idx, int startRow, int endRow)
        {
            Set(idx, start, end, true);

            for (int i = startRow; i < endRow + 1; i++) 
                yield return state[i, idx];
        }

        public IEnumerable<T> SelectCol(int idx)
        {
            return SelectCol(idx, state.MinRow, state.MaxRow);
        }

        public void Dispose()
        {
            state = null;
        }

        public bool MoveNext()
        {
            position++;
            return (position < end + 1);
        }

        public void Reset()
        {
            Set(0, 0, 0, false);
        }

        public UnidimensionalSlice(MultidimensionalArray<T> arr)
        {
            this.state = arr;
        }
    }
}
