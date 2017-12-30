using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskMonaSharp.Models
{
    public class Response
    {
        public int r_id { get; set; }
        public int state { get; set; }
        public int created { get; set; }
        public int u_id { get; set; }
        public string u_name { get; set; }
        public string u_dan { get; set; }
        public string u_times { get; set; }
        public string receive { get; set; }
        public int res_lv { get; set; }
        public int rec_count { get; set; }
        public string response { get; set; }
    }
}
