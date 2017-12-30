using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskMonaSharp.Models
{
    public class TopicList
    {
        public int status { get; set; }
        public int count { get; set; }
        public List<Topic> topics { get; set; }
    }
}
