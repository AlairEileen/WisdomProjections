using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomProjections.Extensions
{
   public static class ListIndexChangeExtension
    {
        //public int IndexOf<T>(this List<T> list,T item) 
        //{
        //    return list.FindIndex(x=>x.Equals(item));
        //}
    }
    public abstract class ListItemIndex
    {
        public int Index { get; set; }
    }
}
