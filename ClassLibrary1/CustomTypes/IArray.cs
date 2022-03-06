using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volatility.CustomTypes
{
    public interface IArray<T>
    {
        T this[int i, int j] { get; set; }
    }
}
