using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Global_MAU.Models
{
    class generateCSV
    {


        public static string GenerateCSVFile<T>(IList<T> list, string name, int storeId)
        {
            if (list == null || list.Count == 0) return "";
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string uploadPath = Path.Combine(exePath, "Upload");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            string filename = $"{name}{storeId}{DateTime.Now:yyyyMMddHHmmss}.csv";
            string filePath = Path.Combine(uploadPath, filename);

            Type t = list[0].GetType();
            string newLine = Environment.NewLine;

            using (var sw = new StreamWriter(filePath))
            {
                object o = Activator.CreateInstance(t);
                PropertyInfo[] props = o.GetType().GetProperties();
                foreach (PropertyInfo pi in props)
                {
                    sw.Write(pi.Name + ",");
                }
                sw.Write(newLine);
                foreach (T item in list)
                {
                    foreach (PropertyInfo pi in props)
                    {
                        string value = Convert.ToString(pi.GetValue(item, null)) ?? "";
                        string safeValue = value.Replace(',', ' ');
                        sw.Write(safeValue + ",");
                    }
                    sw.Write(newLine);
                }
            }

            return filename;
        }
        public static string GenerateCSV1(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                int count = 1;
                int totalColumns = dt.Columns.Count;
                foreach (DataColumn dr in dt.Columns)
                {
                    sb.Append(dr.ColumnName);

                    if (count != totalColumns)
                    {
                        sb.Append(",");
                    }

                    count++;
                }
                sb.AppendLine();

                string value = String.Empty;
                foreach (DataRow dr in dt.Rows)
                {
                    for (int x = 0; x < totalColumns; x++)
                    {
                        value = dr[x].ToString();

                        if (value.Contains(",") || value.Contains("\""))
                        {
                            value = '"' + value.Replace("\"", "\"\"") + '"';
                        }

                        sb.Append(value);

                        if (x != (totalColumns - 1))
                        {
                            sb.Append(",");
                        }
                    }

                    sb.AppendLine();
                }
                string filename = "Product" + clsSettings.StoreID + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv";

                File.WriteAllText("Upload\\" + filename, sb.ToString());

                return filename;
            }
            catch (Exception)
            {

            }
            return "";
        }
        public static string GenerateCSV2(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                int count = 1;
                int totalColumns = dt.Columns.Count;
                foreach (DataColumn dr in dt.Columns)
                {
                    sb.Append(dr.ColumnName);

                    if (count != totalColumns)
                    {
                        sb.Append(",");
                    }

                    count++;
                }

                sb.AppendLine();

                string value = String.Empty;
                foreach (DataRow dr in dt.Rows)
                {
                    for (int x = 0; x < totalColumns; x++)
                    {
                        value = dr[x].ToString();

                        if (value.Contains(",") || value.Contains("\""))
                        {
                            value = '"' + value.Replace("\"", "\"\"") + '"';
                        }

                        sb.Append(value);

                        if (x != (totalColumns - 1))
                        {
                            sb.Append(",");
                        }
                    }

                    sb.AppendLine();
                }
                string FullName = "FullName" + clsSettings.StoreID + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv";

                File.WriteAllText("Upload\\" + FullName, sb.ToString());

                return FullName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }
    }
}
