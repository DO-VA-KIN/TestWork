using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class Funcs<T>
    {
        public static void MoveToFirst(ref List<T> list,  T a1)
        {
            list[list.IndexOf(a1)] = list[0];
            list[0] = a1;
        }
    }
}
