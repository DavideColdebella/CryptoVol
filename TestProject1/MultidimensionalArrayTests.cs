using Microsoft.VisualStudio.TestTools.UnitTesting;
using Volatility.CustomTypes;

namespace TestProject1
{
    [TestClass]
    public class MultidimensionalArrayTests
    {

        /// <summary>
        /// Test that read/write is the same as for a square array
        /// </summary>
        [TestMethod]
        public void TestMultidimensonalSquareArray()
        {
            int n = 2;
            var customSquare = new MultidimensionalArray<double>(n, n);
            double[,] arr = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                customSquare[i, i] = i;
                arr[i, i] = i;
            }

            for (int i = 0; i < n; i++)
                Assert.AreEqual(customSquare[i, i], arr[i, i]);

        }

        /// <summary>
        /// Test that read/write is the same as for a rectangular array
        /// </summary>
        [TestMethod]
        public void TestMultidimensonalRectangularArray()
        {
            int nmbRows = 10;
            int nmbCols = 2;
            var customRect = new MultidimensionalArray<double>(nmbRows, nmbCols);
            double[,] arr = new double[nmbRows, nmbCols];

            for (int i = 0; i < nmbCols; i++)
            {
                customRect[i, i] = 15;
                arr[i, i] = 15;
            }
            arr[nmbRows - 1, nmbCols - 1] = 100;
            customRect[nmbRows - 1, nmbCols - 1] = 100;
            Assert.AreEqual(customRect[nmbRows - 1, nmbCols - 1], arr[nmbRows - 1, nmbCols - 1]);
        }

        /// <summary>
        /// Test that if we set data on a reference slice it gets changed also on the primary
        /// </summary>
        [TestMethod]
        public void TestReferenceCopyColumnSliceSetMethod()
        {
            int nmbRows = 10;
            int nmbCols = 2;
            var customArr = new MultidimensionalArray<double>(nmbRows, nmbCols);
            double[,] arr = new double[nmbRows, nmbCols];
            
            // test extension methods
            for(int j = 0; j < nmbCols; j++)
            {
                customArr.Col(j, x => j * 10 + j);

                for (int i = 0; i < nmbRows; i++)
                {
                    arr[i, j] = j * 10 + j;
                    Assert.AreEqual(customArr[i, j], arr[i, j]);
                }

            }
        }

        /// <summary>
        /// Test that if we set data on a reference slice it gets changed also on the primary
        /// </summary>
        [TestMethod]
        public void TestReferenceCopyRowSliceSetMethod()
        {
            int nmbRows = 10;
            int nmbCols = 2;
            var idxRow = 1;

            var customArr = new MultidimensionalArray<double>(nmbRows, nmbCols);
            double[,] arr = new double[nmbRows, nmbCols];

        }
    }
}
