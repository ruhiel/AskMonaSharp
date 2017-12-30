using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskMonaSharp.Attribute
{
    [AttributeUsageAttribute(AttributeTargets.Method, AllowMultiple = false)]
    public class QueryAttribute : System.Attribute
    {
        public Method Method { get; private set; }
        public string Query { get; private set; }

        public QueryAttribute(Method method, string query)
        {
            Method = method;
            Query = query;
        }
    }
}
