using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global_MAU.Models
{
    class clsFTPsettings
    {
        public string StoreID { set; get; }
        public int stockeditems { set; get; }
        public int NoWebSale { set; get; }
        public string server { set; get; }
        public string userid { set; get; }
        public string pwd { set; get; }
        public string upfolder { set; get; }
        public string Tax { set; get; }
        public decimal MarkUpPrice { set; get; }
        public decimal Deposit { set; get; }

        public string POSName { get; set; }
    }
}
