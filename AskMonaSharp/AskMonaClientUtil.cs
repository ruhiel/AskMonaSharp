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

        public static int? ToValue(this bool? b) => b.HasValue ? (b.Value ? 1 : 0) : (int?)null;

        public static string ToValue(this Order? order) => order.HasValue ? order.Value.ToString().ToLower() : null;

        public static Dictionary<string, object> Compact(this object obj) => obj.GetType().GetProperties()
                .Where(x => x.CanRead && x.GetValue(obj) != null)
                .ToDictionary(pi => pi.Name, pi => pi.GetValue(obj));
    }

}
