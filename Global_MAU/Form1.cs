using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Global_MAU.Models;
using Newtonsoft.Json;


namespace Global_MAU
{
    public partial class BottleCapps : Form
    {
        DataTable dtCat;

        private static string Argsprams { get; set; }

         
        
        public BottleCapps(string[] args)
        {
            InitializeComponent();
            if (args.Length > 0)
            {
                Argsprams = args[0];
            }
            else
            {
                Argsprams = "";
            }
        }
        private void FormLoad(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {

            
            // Ensure folders exist
            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configPath = Path.Combine(exeDir, "config");
            if (!Directory.Exists(configPath)) Directory.CreateDirectory(configPath);

            string uploadPath = Path.Combine(exeDir, "Upload");
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            // Auto-run if Argsprams is set
            if (!string.IsNullOrEmpty(Argsprams))
            {
                Uploading();
                Environment.Exit(0);
            }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            FormSetting frmSettings = new FormSetting();
            frmSettings.ShowDialog();
            Cursor.Current = Cursors.Default;
        }
        public void showstatus(string str)
        {
            var itm = str;
            listBox1.Items.Add(itm);
            listBox1.Refresh();
        }
        private void btnUpload_Click(object sender, EventArgs e)
        {
            Uploading();
        }

        private string SqlSafeColumn(string colName, string defaultLiteral)
        {
            return string.IsNullOrWhiteSpace(colName) || colName.Trim().ToUpper() == "N/A"
                ? defaultLiteral
                : colName;
        }


        // future reference 
        private string SqlSafeNumericColumn(string colName)
        {
            // For INT, DECIMAL, FLOAT, etc.
            return string.IsNullOrWhiteSpace(colName) || colName.Trim().ToUpper() == "N/A"
                ? "0"
                : colName;
        }
        private string SqlSafeTableName(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName) || tableName.Trim().ToUpper() == "N/A")
                throw new InvalidOperationException("Table name is missing in settings.");
            return tableName;
        }
        private void Uploading()
        {
            // Ensure selectedPOS
            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configPath = Path.Combine(exeDir, "config");

           

            // Settings
            clsSettings.LoadSttings();
            showstatus("Connecting to Database");

            // POS-specific categories file (first-run safe)
            string catsFile = Path.Combine(configPath, "cats.txt");
            if (!File.Exists(catsFile)) File.Create(catsFile).Close();
          
            string jsoncats = File.ReadAllText(catsFile, Encoding.UTF8);
           
            var fileStream = new FileStream(@"config\cats.txt", FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                jsoncats = streamReader.ReadToEnd();
            }
            string strcats = "";    // this is for depopsit flags   making global from locol
            string strcats1 = "";
            List<depositCategory> depositCategories = new List<depositCategory>();
            if (!string.IsNullOrEmpty(jsoncats))
            {
                clsCategories[] clscat = JsonConvert.DeserializeObject<clsCategories[]>(jsoncats);
               
                // string strcats = "";    // this is for depopsit flags  locol
              
                    foreach (clsCategories cat in clscat)
                    {
                        if (cat.catname == null) cat.catname = "";
                        if (cat.sel==1) 
                        {
                            depositCategory deposits = new depositCategory();
                            if (cat.dep == 1)
                            {
                                deposits.catname = cat.catname;
                                depositCategories.Add(deposits);
                            }
                            if (cat.catname.Contains("'"))
                                cat.catname = cat.catname.Replace("'", "''");
                            if (strcats.Length > 0)
                                strcats += ",'" + cat.catname + "'";
                            else
                                strcats += "'" + cat.catname + "'";
                        }
                    }
                    strcats1 = strcats;
                
                }
                if (!string.IsNullOrWhiteSpace(clsSettings.pcat) && !clsSettings.pcat.Equals("N/A", StringComparison.OrdinalIgnoreCase))
            {
                if (strcats.Length > 0)
                {
                   strcats = " AND " + clsSettings.pcat + " IN (" + strcats1 + ")";  
                }
            }
            else
            {
                strcats = "";
            }

            string strStock = "";

            string servername = System.Environment.MachineName.ToString();
            string connectionstring = clsSettings.connectionString;
            string storeid = clsSettings.StoreID;
            string tax = clsSettings.Tax;
            decimal markup = clsSettings.MarkUpPrice;
            decimal deposit = clsSettings.Deposit;


            if (clsSettings.uom.Equals("N/A"))
                clsSettings.uom = "''";
            if (clsSettings.pack.Equals("N/A"))
                clsSettings.pack = "''";
            if (clsSettings.cost.Equals("N/A"))
                clsSettings.cost = "''";
            if (clsSettings.sprice.Equals("N/A"))
                clsSettings.sprice = "''";
            if (clsSettings.altupc1.Equals("N/A"))
                clsSettings.altupc1 = "''";
            if (clsSettings.altupc2.Equals("N/A"))
                clsSettings.altupc2 = "''";
            if (clsSettings.altupc3.Equals("N/A"))
                clsSettings.altupc3 = "''";
            if (clsSettings.altupc4.Equals("N/A"))
                clsSettings.altupc4 = "''";
            if (clsSettings.altupc5.Equals("N/A"))
                clsSettings.altupc5 = "''";
            if (clsSettings.Start.Equals("N/A"))
                clsSettings.Start = "''";
            if (clsSettings.End.Equals("N/A"))
                clsSettings.End = "''";
            if (clsSettings.pcat.Equals("N/A"))
                clsSettings.pcat = "''";
            if (clsSettings.pcat1.Equals("N/A"))
                clsSettings.pcat1 = "''";
            if (clsSettings.pcat2.Equals("N/A"))
                clsSettings.pcat2 = "''";
            if (clsSettings.vintage.Equals("N/A"))
                clsSettings.vintage = "''";
            if (clsSettings.Discountable.Equals("N/A"))
                clsSettings.Discountable = "''";
            if (clsSettings.Condition.Equals("N/A"))
                clsSettings.Condition = "";
            clsSettings.country = "''";
            clsSettings.region = "''";
            if (string.IsNullOrEmpty(clsSettings.Condition) && clsSettings.stockeditems == 1)
                strStock = " and " + clsSettings.Qty + " > 0 ";
            else if (!string.IsNullOrEmpty(clsSettings.Condition) && clsSettings.stockeditems == 1)
                strStock = " and " + clsSettings.Qty + " > 0 ";
            if (string.IsNullOrEmpty(clsSettings.Condition) && clsSettings.stockeditems == 0)
            {
                if (clsSettings.pcat.Equals("''"))
                    strcats = "";
                else if (strcats.Length > 0)
                    strcats = " and " + clsSettings.pcat + " in (" + strcats1 + ")";
            }
            StringBuilder cmdstring = new StringBuilder();

            cmdstring.AppendLine(" SELECT DISTINCT " + storeid + " AS storeid, ");


            /*
              // added these to for posnation/caps  pos   but throwing error for  EBS
              
              cmdstring.AppendLine(" CASE WHEN LEN(ISNULL(CAST(" + clsSettings.upc + " AS VARCHAR(50)), '')) > 2 " +          // these 4 brackets might be the error
                                    " THEN '#' + REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(ISNULL(CAST(" + clsSettings.upc + " AS VARCHAR(50)), '')))), '*', ''), '/', ''), ' ', '') " +
                                    " ELSE '' END AS upc, ") ;

             // Console.WriteLine(cmdstring.ToString());


              cmdstring.AppendLine(" CASE WHEN LEN(ISNULL(CAST(" + clsSettings.sku + " AS VARCHAR(50)), '')) > 0 "  +                   // these 4 brackets might be the error
                                   " THEN '#' + REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(ISNULL(CAST(" + clsSettings.sku + " AS VARCHAR(50)), '')))), '*', ''), '/', ''), ' ', '') " +
                                   " ELSE ''END AS sku, ");    
               */

            cmdstring.AppendLine(" CASE WHEN LEN(ISNULL(CAST(" + clsSettings.upc + " AS VARCHAR(50)), '')) > 2 " +
                                 " THEN '#' + REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(ISNULL(CAST(" + clsSettings.upc + " AS VARCHAR(50)), ''))), '*', ''), '/', ''), ' ', '') "  + "  ELSE '' END AS upc, ");

            cmdstring.AppendLine(" CASE WHEN LEN(ISNULL(CAST(" + clsSettings.sku  + " AS VARCHAR(50)), '')) > 0 " +
                               " THEN '#' + REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(ISNULL(CAST(" + clsSettings.sku + " AS VARCHAR(50)), ''))), '*', ''), '/', ''), ' ', '')" + " ELSE '' END AS sku, ");



            if (clsSettings.StaticQuantity)
                cmdstring.AppendLine(" " + clsSettings.StaticQtyvalue + " as qty, ");

            // common for every store where if quantity is negative  it becomes 0  
            /*else
            {
                cmdstring.AppendLine(" CASE WHEN ISNULL(TRY_CAST(" + clsSettings.Qty + " AS INT),0) < 0 " +
                         " THEN 0 ELSE ISNULL(TRY_CAST(" + clsSettings.Qty + " AS INT),0) END AS qty, ");
            }*/

            // Take quantity as it is (-ve,+ve, 0 ) (added for 12499)
            else
            {

                cmdstring.AppendLine(" ISNULL(TRY_CAST(" + clsSettings.Qty + " AS INT), 0) AS qty, ");
            }
            cmdstring.AppendLine(" CASE WHEN " + clsSettings.pack + " IS NULL THEN 1 ELSE " + clsSettings.pack + " END AS pack, ");
            cmdstring.AppendLine(" CASE WHEN " + clsSettings.uom + " IS NULL THEN '' ELSE " + clsSettings.uom + " END AS uom, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.StoreProductName + ", '') AS StoreProductName, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.StoreDescription + ", '') AS StoreDescription, ");
            cmdstring.AppendLine(" CAST(ISNULL(TRY_CAST(" + clsSettings.Price + " AS DECIMAL(18,2)),0) " +
                         " + ISNULL(TRY_CAST(" + clsSettings.Price + " AS DECIMAL(18,2)),0) * (" + markup + " / 100) " +
                         " AS DECIMAL(18,2)) AS Price, ");
            cmdstring.AppendLine(" CASE WHEN ISNULL(TRY_CAST(" + clsSettings.sprice + " AS DECIMAL(18,2)),0) >= " +
                         " ISNULL(TRY_CAST(" + clsSettings.Price + " AS DECIMAL(18,2)),0) " +
                         " THEN 0 ELSE ISNULL(TRY_CAST(" + clsSettings.sprice + " AS DECIMAL(18,2)),0) END AS Sprice, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.Start + ", '') AS Startdate, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.End + ", '') AS EndDate, ");
            cmdstring.AppendLine(" " + clsSettings.Tax + " AS tax, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.altupc1 + ", '') AS altupc1, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.altupc2 + ", '') AS altupc2, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.altupc3 + ", '') AS altupc3, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.altupc4 + ", '') AS altupc4, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.altupc5 + ", '') AS altupc5, ");
            cmdstring.AppendLine(" " + deposit + " AS deposit, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.vintage + ", '') AS Vintage, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.pcat + ", '') AS pcat, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.pcat1 + ", '') AS pcat1, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.pcat2 + ", '') AS pcat2, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.country + ", '') AS country, ");
            cmdstring.AppendLine(" ISNULL(" + clsSettings.region + ", '') AS region, ");
            cmdstring.AppendLine(" CASE WHEN " + clsSettings.Discountable + " IS NULL THEN 0 ELSE " + clsSettings.Discountable + " END AS Discountable,  ");
            cmdstring.AppendLine(" CAST(ISNULL(TRY_CAST(" + clsSettings.cost + " AS DECIMAL(18,2)),0) " +
                         " + ISNULL(TRY_CAST(" + clsSettings.cost + " AS DECIMAL(18,2)),0) " +
                         " AS DECIMAL(18,2)) AS Cost ");
            cmdstring.AppendLine(" FROM " + clsSettings.TableName + "  ");
            cmdstring.AppendLine(" WHERE " + clsSettings.Price + " > 0 ");
            if (!string.IsNullOrEmpty(clsSettings.Condition))
                cmdstring.AppendLine(" and " + " " + clsSettings.Condition +  strStock + strcats);   
            else if ((string.IsNullOrEmpty(strcats) && string.IsNullOrEmpty(strStock)))
                cmdstring.AppendLine("");
            else
            {

                cmdstring.AppendLine(" and " + " " + strStock + strcats);
            }



            // Execute query
            string test = cmdstring.ToString();
            DataTable dtresult = new DataTable();
            using (SqlConnection con = new SqlConnection(clsSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand(cmdstring.ToString(), con))
            using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
            {
                cmd.CommandTimeout = 0;
                adp.Fill(dtresult);
            }
            List<ProductModel> pdf = new List<ProductModel>();
            List<FullNameModel> fnf = new List<FullNameModel>();
            // Process results
            foreach (DataRow dt in dtresult.Rows)
            {
                ProductModel pd = new ProductModel();
                FullNameModel fn = new FullNameModel();
                pd.StoreID = Convert.ToInt32(clsSettings.StoreID);
                

                pd.upc = Regex.Replace(dt["upc"].ToString().Trim(), @"[^#0-9A-Za-z]", "");
                pd.sku = Regex.Replace(dt["sku"].ToString().Trim(), @"[^#0-9A-Za-z]", "");
                if (String.IsNullOrEmpty(pd.upc))
                {
                    continue;
                }
                pd.Qty = Convert.ToInt32(dt["qty"]);
                if (storeid=="12499")
                {
                    //pd.Qty =Convert.ToInt32( Regex.Replace(pd.Qty.ToString(), @"-", "") );  
                    pd.Qty = Math.Abs(pd.Qty);   // -ve to  +ve 
                }
                pd.StoreProductName = dt["StoreProductName"].ToString();
                pd.StoreDescription = dt["StoreProductName"].ToString();

                pd.pack = Convert.ToInt32(dt["pack"].ToString());
                if (pd.pack==1 || pd.pack ==0)
                {
                    pd.pack = getpack(dt["StoreProductName"].ToString());
                }
                pd.uom = dt["uom"].ToString();
                if (string.IsNullOrEmpty(pd.uom) || pd.uom == "0")
                {
                    pd.uom = getVolume(dt["StoreProductName"].ToString());
                }
                pd.Price = Convert.ToDecimal(dt["price"]);
                pd.sprice = Convert.ToDecimal(dt["Sprice"]);
                if (pd.sprice > 0)
                {
                    pd.Start = dt["startdate"].ToString();
                    pd.End = dt["enddate"].ToString();
                }
                if (clsSettings.QtyPerPack)
                    pd.Qty = pd.Qty / pd.pack; 

                pd.Tax = Convert.ToDecimal(clsSettings.Tax);
                pd.altupc1 = dt["altupc1"].ToString();
                pd.altupc2 = dt["altupc2"].ToString();
                pd.altupc3 = dt["altupc3"].ToString();
                pd.altupc4 = dt["altupc4"].ToString();
                pd.altupc5 = dt["altupc5"].ToString();

                pd.deposit = depositCategories.Any(x => x.catname == dt["pcat"].ToString())? Convert.ToDecimal(dt["deposit"]): 0;
                //  pd.deposit = Convert.ToDecimal(dt["deposit"]);

                //fullname file 

                /*fn.upc = dt["upc"].ToString();
                fn.sku = dt["sku"].ToString().Trim();*/

                fn.upc = Regex.Replace(dt["upc"].ToString().Trim(), @"[^#0-9A-Za-z]", "");
                fn.sku = Regex.Replace(dt["sku"].ToString().Trim(), @"[^#0-9A-Za-z]", "");
                if (String.IsNullOrEmpty(fn.upc))
                {
                    continue;
                }
                fn.pname = pd.StoreProductName;
                fn.pdesc = pd.StoreDescription;
                fn.Price = Convert.ToDecimal(dt["price"]);
                fn.pack = pd.pack;
                fn.uom = pd.uom;
                fn.pcat = dt["pcat"].ToString();
                fn.pcat1 = dt["pcat1"].ToString();
                fn.pcat2 = dt["pcat2"].ToString();
                fn.region = dt["region"].ToString();
                fn.country = dt["country"].ToString();

                if (pd.Price > 0)
                {
                    pdf.Add(pd);
                    fnf.Add(fn);
                }

            }

            showstatus("Generating csv file");
            string productFile = generateCSV.GenerateCSVFile(pdf, "Product", Convert.ToInt32(clsSettings.StoreID));
            string fullnameFile = generateCSV.GenerateCSVFile(fnf, "FullName", Convert.ToInt32(clsSettings.StoreID));

            showstatus($"Generated: {productFile} and {fullnameFile}");

            showstatus("Uploading " + productFile);
            showstatus("Uploading " + fullnameFile);

            Upload("Upload//" + productFile);
            Upload("Upload//" + fullnameFile);

            showstatus("Upload completed");



            string prodPath = Path.Combine("Upload", productFile);
            string fullPath = Path.Combine("Upload", fullnameFile);
            if (File.Exists(prodPath)) File.Delete(prodPath);
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
        private void Upload(string filename)
        {

            // Validate local file
            FileInfo fileInf = new FileInfo(filename);
            if (!fileInf.Exists)
            {
                MessageBox.Show($"Local file not found: {fileInf.FullName}", "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate FTP settings
            if (string.IsNullOrWhiteSpace(clsSettings.FTPserver) ||
                string.IsNullOrWhiteSpace(clsSettings.FTPuserid) ||
                string.IsNullOrWhiteSpace(clsSettings.FTPpwd) ||
                string.IsNullOrWhiteSpace(clsSettings.FTPupfolder))
            {
                MessageBox.Show("FTP settings are incomplete. Please configure them before uploading.",
                                "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Normalize and encode remote folder
            string remoteFolder = clsSettings.FTPupfolder.Trim().Trim('/');
            string encodedFolder = Uri.EscapeDataString(remoteFolder);
            string encodedFileName = Uri.EscapeDataString(fileInf.Name);

            // Build FTP URI
            string ftpUri = $"ftp://{clsSettings.FTPserver}/{encodedFolder}/{encodedFileName}";

            try
            {
                FtpWebRequest reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(ftpUri));
                reqFTP.Credentials = new NetworkCredential(clsSettings.FTPuserid, clsSettings.FTPpwd);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UseBinary = true;
                reqFTP.ContentLength = fileInf.Length;

                byte[] buffer = new byte[2048];
                int bytesRead;

                using (FileStream fs = fileInf.OpenRead())
                using (Stream strm = reqFTP.GetRequestStream())
                {
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        strm.Write(buffer, 0, bytesRead);
                    }
                }

                showstatus($"Uploaded: {fileInf.Name} → {remoteFolder}");
            }
            catch (WebException wex)
            {
                string message;
                if (wex.Response is FtpWebResponse ftpResponse)
                {
                    message = $"FTP Error: {ftpResponse.StatusDescription}";
                }
                else
                {
                    message = wex.Message;
                }
                MessageBox.Show(message, "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public int getpack(string prodName)
        {
            prodName = prodName.ToUpper();
            var regexMatch = Regex.Match(prodName, @"(?<Result>\d+)PK");
            var prodPack = regexMatch.Groups["Result"].Value;
            if (prodPack.Length > 0)
            {
                return ParseIntValue(prodPack);
            }
            return 1;
        }
        public string getVolume(string prodName)
        {
            prodName = prodName.ToUpper();
            var regexMatch = Regex.Match(prodName, @"(?<Result>\d+)ML| (?<Result>\d+)LTR| (?<Result>\d+)OZ | (?<Result>\d+)L");
            var prodPack = regexMatch.Groups["Result"].Value;
            if (prodPack.Length > 0)
            {
                return regexMatch.ToString();
            }
            return "";
        }
        public int ParseIntValue(string val)
        {
            int outVal = 0;
            int.TryParse(val.Replace("$", ""), out outVal);
            return outVal;
        }
        private void Emptytxt()
        {
            clsDBsettings clsdb = new clsDBsettings();
            clsdb.server = "";
            clsdb.userid = "";
            clsdb.pwd = "";
            clsdb.database = "";
            clsdb.IntegratedSecurity = false;

            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            using (StreamWriter sw = new StreamWriter(@"config\dbsettings.txt"))
            using (Newtonsoft.Json.JsonTextWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                serializer.Serialize(writer, clsdb);
                sw.Close();
                writer.Close();
            }

            clsFTPsettings clsftp = new clsFTPsettings();
            clsftp.StoreID = "";
            clsftp.server = "";
            clsftp.userid = "";
            clsftp.pwd = "";
            clsftp.upfolder = "";
            clsftp.Tax = "";
            clsftp.stockeditems = 0;
            clsftp.MarkUpPrice = 0;
            clsftp.Deposit = 0;

            using (StreamWriter sw = new StreamWriter(@"config\ftpsettings.txt"))
            using (Newtonsoft.Json.JsonTextWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                serializer.Serialize(writer, clsftp);
                sw.Close();
                writer.Close();
            }



            using (StreamWriter sw = new StreamWriter(@"config\cats.txt"))
            using (Newtonsoft.Json.JsonTextWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                serializer.Serialize(writer, "");
                sw.Close();
                writer.Close();
            }
        }


        private void Query_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
           
            Cursor.Current = Cursors.Default;
        }


    }
}








