using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e_Solution
{
    public class eQuestion
    {
        public int id { get; set; }
        public int importance { get; set; }
        public Boolean urgent { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
        public string textColor { get; set; }
        public DateTime date { get; set; }
        public string user { get; set; }
    }
}
