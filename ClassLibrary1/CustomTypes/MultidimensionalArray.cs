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
    
    public class MultidimensionalArray<T> : IArray<T>
    {
        /// <summary>
        /// Rectangular boundaries
        /// </summary>
        public int MinRow = 0;
        public int MaxRow;
        public int MinCol = 0;
        public int MaxCol;

        private T[,] state;
        public int NmbRows { get; private set; }
        public int NmbCols { get; private set; }
        //private delegate T AccessElementAt(int rowI, int colJ);
        //private delegate void SetElementAt(int rowI, int colJ, T value);
        //private AccessElementAt At;
        //private SetElementAt SetAt;
        //public T ElementAt(int rowI, int colJ) => this.At(rowI, colJ);
        //public void ElementAt(int rowI, int colJ, T value)
        //{
        //    SetAt(rowI, colJ, value);
        //}
        private MultidimensionalArray() { }

        public MultidimensionalArray(int nmbRows, int nmbCols)
        {
            state = new T[nmbRows, nmbCols];
            NmbRows = nmbRows;
            NmbCols = nmbCols;
            MaxRow = nmbRows - 1;
            MaxCol = nmbCols - 1;
            //At = (rowI, colJ) => this[rowI, colJ];
            //SetAt = (rowI, colJ, value) => { this[rowI, colJ] = value; };
        }

        public T this[int rowI, int colJ]
        {
            get
            {
                // with safe access
                if (rowI < MinRow || rowI > MaxRow || colJ < MinCol || colJ > MaxCol)
                    throw new ArgumentOutOfRangeException($"Ensure for cols {MinRow}<={rowI}<={MaxRow} and for rows {MinCol}<=i:{colJ}<={MaxCol}");
                return state[rowI, colJ]; 
            }
            set 
            {
                // with safe access
                if (rowI < MinRow || rowI > MaxRow || colJ < MinCol || colJ > MaxCol)
                    throw new ArgumentOutOfRangeException($"Ensure for cols {MinRow}<={rowI}<={MaxRow} and for rows {MinCol}<=i:{colJ}<={MaxCol}"); 
                state[rowI, colJ] = value; 
            }
        }

        public ISlice<T> Slice()
        {
            return new Slice<T>(this);
        }

        //public MultidimensionalArray<T> GetCol(int idxCol)
        //{
        //    var multidimArr = new MultidimensionalArray<T>();
        //    multidimArr.state = this.state;
        //    multidimArr.NmbRows = this.NmbRows;
        //    multidimArr.NmbCols = 1;
        //    multidimArr.MinCol = 0;
        //    multidimArr.MaxCol = 0;
        //    multidimArr.MinRow = this.MinRow;
        //    multidimArr.MaxRow = this.MaxRow;

        //    multidimArr.At = (i, j) => this[i, j + idxCol];
        //    multidimArr.SetAt = (i, j, value) => { this[i, j + idxCol] = value; };
        //    return multidimArr;
        //}

        //public MultidimensionalArray<T> GetRow(int idxRow)
        //{
        //    var multidimArr = new MultidimensionalArray<T>();
        //    multidimArr.state = this.state;
        //    multidimArr.NmbRows = 1;
        //    multidimArr.NmbCols = this.NmbCols;
        //    multidimArr.MinCol = this.MinCol;
        //    multidimArr.MaxCol = this.MaxCol;
        //    multidimArr.MinRow = 0;
        //    multidimArr.MaxRow = 0;

        //    multidimArr.At = (i, j) => this[i + idxRow, j];
        //    multidimArr.SetAt = (i, j, value) => { this[i + idxRow, j] = value; };
        //    return multidimArr;
        //}
    }

    /// <summary>
    /// Projection?
    ///  - https://stackoverflow.com/questions/1160217/changing-element-value-in-listt-foreach-foreach-method
    /// </summary>
    public static class Extension
    {
        public static IEnumerable<T> Col<T>(this MultidimensionalArray<T> arr, int idxCol)
        {
            return arr.Slice().ColEnumerator(idxCol);
        }

        /// <summary>
        /// Sets elements on the whole column. How to allow arbitrary starting point?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="idxCol"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public static IEnumerable<T> Col<T>(this MultidimensionalArray<T> arr, int idxCol, Func<T, T> projection)
        {
            return arr.Slice().ColEnumerator(idxCol, projection);
        }
    }


    /// <summary>
    /// Safe & efficient code
    ///  - https://docs.microsoft.com/en-us/dotnet/csharp/write-safe-efficient-code
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISlice<T>
    {
        Func<int, T> ColIteratorGet(int idxCol);
        Func<int, T> RowIteratorGet(int idxRow);
        Action<int, T> ColIteratorSet(int idxCol);
        Action<int, T> RowIteratorSet(int idxRow);
        IEnumerable<T> ColEnumerator(int idxCol);
        IEnumerable<T> ColEnumerator(int idxCol, Func<T, T> projection);
        void Execute(IEnumerable<T> instructions);
    }

    public class Slice<T>: ISlice<T>
    {
        private MultidimensionalArray<T> state;
        private int startCol;
        private int endCol;
        private int startRow;
        private int endRow;

        public void Execute(IEnumerable<T> instructions)
        {
            var it = instructions.GetEnumerator();
            _ = it.MoveNext();
            for (int i = startRow; i < endRow + 1; i++)
            {
                for (int j = startCol; i < endCol + 1; i++)
                    state[i, j] = it.Current;
                _ = it.MoveNext();
            }
        }

        private IEnumerable<T> ColEnumerator(int idxCol, int startRow, int endRow, Func<T, T> projection)
        {
            this.startCol = idxCol;
            this.endCol = idxCol;
            this.startRow = startRow;
            this.endRow = endRow;

            for (int i = startRow; i < endRow + 1; i++)
            {
                yield return projection(state[i, idxCol]);
            }
        }
        public IEnumerable<T> ColEnumerator(int idxCol, Func<T, T> projection)
        {
            return ColEnumerator(idxCol, state.MinRow, state.MaxRow, projection);
        }

        private IEnumerable<T> ColEnumerator(int idxCol, int startRow, int endRow)
        {
            for (int i = startRow; i < endRow + 1; i++) 
                yield return state[i, idxCol];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idxCol"></param>
        /// <returns></returns>
        /// <remarks>Not tested</remarks>
        public IEnumerable<T> ColEnumerator(int idxCol)
        {
            return ColEnumerator(idxCol, state.MinRow, state.MaxRow);
        }

        public Func<int, T> ColIteratorGet(int idxCol)
        {
            return (i) => state[i, idxCol];
        }

        public Func<int, T> RowIteratorGet(int idxRow)
        {
            return (i) => state[idxRow, i];
        }

        public Action<int, T> ColIteratorSet(int idxCol)
        {
            return (i, val) => state[i, idxCol] = val;
        }

        public Action<int, T> RowIteratorSet(int idxRow)
        {
            return (i, val) => state[idxRow, i] = val;
        }

        public Slice(MultidimensionalArray<T> arr)
        {
            state = arr;
        }
    }
}
