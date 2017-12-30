using AskMonaSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskMonaSharp
{
    public static class AskMonaClientUtil
    {
        public static int? ToValue(this Category? category) => category.HasValue ? (int?)category.Value : null;

        public static Dictionary<string, object> Compact(this object obj) => obj.GetType().GetProperties()
                .Where(x => x.CanRead)
                .ToDictionary(pi => pi.Name, pi => pi.GetValue(obj));
    }

}
