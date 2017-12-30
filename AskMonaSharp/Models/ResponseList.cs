using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskMonaSharp.Models
{
    public class ResponseList
    {
        public int status { get; set; }
        public int updated { get; set; }
        public int modified { get; set; }
        public List<Response> responses { get; set; }
    }
}
