using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global_MAU.Models
{
   public class clsQuery
    {
        public string upc { get; set; }
        public string Qty { get; set; }
        public string sku { get; set; }
        public string pack { get; set; }
        public string uom { get; set; }
        public string StoreProductName { get; set; }
        public string StoreDescription { get; set; }
        public string Price { get; set; }
        public string sprice { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Tax { get; set; }
        public string altupc1 { get; set; }
        public string altupc2 { get; set; }
        public string altupc3 { get; set; }
        public string altupc4 { get; set; }
        public string altupc5 { get; set; }
        public string Discountable { get; set; }
        public string pcat { get; set; }
        public string pcat1 { get; set; }
        public string pcat2 { get; set; }
        public string vintage { get; set; }
        public string country { get; set; }
        public string region { get; set; }
        public string StaticQtyvalue { get; set; }
        public string TableName { get; set; }
        public string Condition { get; set; }
        public bool StaticQuantity { set; get; }
        public bool QtyPerPack { set; get; }

        public string cost { set; get; }

        //public string Active_Pos_Name { set; get; }


    }

    public class MasterQuery
    {
        public string PosName { get; set; }

        public clsQuery clsQuery { get; set; }
    }
}
