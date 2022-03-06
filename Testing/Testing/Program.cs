using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volatility.CustomTypes;

namespace Volatility.Testing
{
    public static class Program
    {
        static void Main()
        {
            var testArr = new MultidimensionalArray<double>(1, 10);
            testArr.ElementAt(0, 9, 10);

            //var copyArr = testArr.GetColArray(9);
            //for(int i =0; i<testArr.NmbRows; i++)
            //    copyArr.ElementAt(i, 0, 5);

            //double[,] test = new double[10, 10];
            //for (int i = 0; i < testArr.NmbRows; i++)
            //    test[0, i] = i;
            //;

        }
    }
}
