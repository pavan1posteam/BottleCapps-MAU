using Global_MAU.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Global_MAU
{
    class clsSettings
    {
        /*public clsSettings()
        {
            clsSettings.LoadSttings();
        }*/
        public static string connectionString = "";
        public static string StoreID { set; get; }
        public static string FTPserver { set; get; }
        public static string FTPuserid { set; get; }
        public static string FTPpwd { set; get; }
        public static string FTPupfolder { set; get; }
        public static int uptime { set; get; }
        public static string DateFrom { set; get; }
        public static string To { set; get; }
        public static string Tax { set; get; }
        public static int stockeditems { set; get; }
        public static string DBserver { set; get; }
        public static string DBuserid { set; get; }
        public static string DBpwd { set; get; }
        public static string DBdatabase { set; get; }
        public static bool IntegratedSecurity { set; get; }
        public static decimal MarkUpPrice { set; get; }
        public static decimal Deposit { set; get; }

        //Query

        public static string upc { get; set; }
        public static string Qty { get; set; }
        public static string sku { get; set; }
        public static string pack { get; set; }
        public static string uom { get; set; }
        public static string StoreProductName { get; set; }
        public static string StoreDescription { get; set; }
        public static string Price { get; set; }
        public static string sprice { get; set; }
        public static string Start { get; set; }
        public static string End { get; set; }
        public static string altupc1 { get; set; }
        public static string altupc2 { get; set; }
        public static string altupc3 { get; set; }
        public static string altupc4 { get; set; }
        public static string altupc5 { get; set; }
        public static string Discountable { get; set; }
        public static string pcat { get; set; }
        public static string pcat1 { get; set; }
        public static string pcat2 { get; set; }
        public static string vintage { get; set; }
        public static string country { get; set; }
        public static string region { get; set; }
        public static string StaticQtyvalue { get; set; }
        public static string TableName { get; set; }
        public static string Condition { get; set; }
        public static bool StaticQuantity { set; get; }
        public static bool QtyPerPack { set; get; }
        public static bool NegativeQTY_to_Positive { set; get; }
        public static bool RoundUpPrice { set; get; }
        public static string cost { set; get; }
        public static string LastPOSName { set; get; }
        public static int LastPOSIndex { get; set; }
        public static List<string> POSList { get; set; } = new List<string>();
        public static void LoadSttings()
        {
            string dbFile = "config/dbsettings.txt";       // Shared DB settings file
            string ftpFile = "config/ftpsettings.txt";     // Shared FTP settings file
            string queryFilePath = "config/Query.txt";     // Shared query file for all POS

            string json;
            // --- Load DB Settings (shared) ---
            if (File.Exists(dbFile))
            {
                json = File.ReadAllText(dbFile, Encoding.UTF8);
                var clsdb = JsonConvert.DeserializeObject<clsDBsettings>(json) ?? new clsDBsettings();
                DBserver = clsdb.server ?? string.Empty;
                DBuserid = clsdb.userid ?? string.Empty;
                DBpwd = clsdb.pwd ?? string.Empty;
                DBdatabase = clsdb.database ?? string.Empty;
                IntegratedSecurity = clsdb.IntegratedSecurity;
                connectionString = IntegratedSecurity
                    ? $"Server={DBserver}; Database={DBdatabase}; Integrated Security=True;"
                    : $"Server={DBserver}; Database={DBdatabase}; User Id={DBuserid}; Password={DBpwd};";
            }
            else
            {
                // No DB file yet → start with defaults
                DBserver = DBuserid = DBpwd = DBdatabase = string.Empty;
                IntegratedSecurity = false;
                connectionString = string.Empty;
            }

            // --- Load FTP Settings (shared) ---
            if (File.Exists(ftpFile))
            {
                json = File.ReadAllText(ftpFile, Encoding.UTF8);
                var clsFTP = JsonConvert.DeserializeObject<clsFTPsettings>(json) ?? new clsFTPsettings();
                StoreID = clsFTP.StoreID ?? string.Empty;
                FTPserver = clsFTP.server ?? string.Empty;
                FTPuserid = clsFTP.userid ?? string.Empty;
                FTPpwd = clsFTP.pwd ?? string.Empty;
                FTPupfolder = clsFTP.upfolder ?? string.Empty;
                Tax = clsFTP.Tax ?? string.Empty;
                stockeditems = clsFTP.stockeditems;
                MarkUpPrice = clsFTP.MarkUpPrice;
                Deposit = clsFTP.Deposit;
                LastPOSName = clsFTP.POSName ?? string.Empty;   // for saving lastposname 
            }
            else
            {
                // No FTP file yet → start with defaults
                StoreID = FTPserver = FTPuserid = FTPpwd = FTPupfolder = Tax = string.Empty;
                stockeditems = 0;
                MarkUpPrice = 0;
                Deposit = 0;
            }

           
            clsQuery query = new clsQuery();
            MasterQuery masterQuery = null;
            if (File.Exists(queryFilePath))
            {


                Dictionary<string, clsQuery> allQueries = JsonConvert.DeserializeObject<Dictionary<string, clsQuery>>(
                    File.ReadAllText(queryFilePath, Encoding.UTF8)
                ) ?? new Dictionary<string, clsQuery>();
                string posKey = LastPOSName?.Trim();
                if (!string.IsNullOrWhiteSpace(posKey) &&  allQueries.TryGetValue(posKey, out clsQuery loadedquery))
                {
                    query = loadedquery ;
                }
            }

            // --- Populate query-related settings ---
            upc = query.upc ?? string.Empty;
            sku = query.sku ?? string.Empty;
            StoreProductName = query.StoreProductName ?? string.Empty;
            StoreDescription = query.StoreDescription ?? string.Empty;
            Qty = query.Qty ?? string.Empty;
            uom = query.uom ?? string.Empty;
            pack = query.pack ?? string.Empty;
            Price = query.Price ?? string.Empty;
            sprice = query.sprice ?? string.Empty;
            Start = query.Start ?? string.Empty;
            End = query.End ?? string.Empty;
            altupc1 = query.altupc1 ?? string.Empty;
            altupc2 = query.altupc2 ?? string.Empty;
            altupc3 = query.altupc3 ?? string.Empty;
            altupc4 = query.altupc4 ?? string.Empty;
            altupc5 = query.altupc5 ?? string.Empty;
            Discountable = query.Discountable ?? string.Empty;
            pcat = query.pcat ?? string.Empty;
            pcat1 = query.pcat1 ?? string.Empty;
            pcat2 = query.pcat2 ?? string.Empty;
            vintage = query.vintage ?? string.Empty;
            country = query.country ?? string.Empty;
            region = query.region ?? string.Empty;
            StaticQtyvalue = query.StaticQtyvalue ?? string.Empty;
            TableName = query.TableName ?? string.Empty;
            Condition = query.Condition ?? string.Empty;
            StaticQuantity = query.StaticQuantity;
            QtyPerPack = query.QtyPerPack;
            cost= query.cost;
            NegativeQTY_to_Positive = query.Negative2Positive;
            RoundUpPrice = query.RoundUpP;
        }



    }
}
