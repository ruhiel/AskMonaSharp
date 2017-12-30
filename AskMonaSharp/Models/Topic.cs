using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskMonaSharp.Models
{
    public class Topic
    {
        public int rank { get; set; }
        public int t_id { get; set; }
        public int u_id { get; set; }
        public int state { get; set; }
        public string title { get; set; }
        public int cat_id { get; set; }
        public string category { get; set; }
        public string tags { get; set; }
        public string lead { get; set; }
        public int created { get; set; }
        public int updated { get; set; }
        public int modified { get; set; }
        public int count { get; set; }
        public string receive { get; set; }
        public int favorites { get; set; }
        public int editable { get; set; }
        public int sh_host { get; set; }
        public string ps { get; set; }
    }

}
