using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global_MAU.Models
{
    class clsDBsettings
    {
        public string server { set; get; }
        public string userid { set; get; }
        public string pwd { set; get; }
        public string database { set; get; }
        public bool IntegratedSecurity { set; get; }
    }
}
