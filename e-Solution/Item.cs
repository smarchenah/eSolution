using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace e_Solution
{
    public class Item
    {
        public int userId { get; set; }
        public string Name { get; set; }
        public string PictureString { get { return "./img/user2.jpg"; } }
    }
}
