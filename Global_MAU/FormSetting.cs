using Global_MAU.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Global_MAU
{
    public partial class FormSetting : Form
    {
        DataTable dtCat;
        public FormSetting()
        {
            InitializeComponent();

        }
        // added on 20-09-25
        string catsFile = "config/cats.txt";
         bool  saveftppop = true ;

        private void FormSettings_Load(object sender, EventArgs e)
        {

            AddHeaderCheckBox();
            HeaderCheckBox.MouseClick += new MouseEventHandler(HeaderCheckBox_MouseClick);
            clsSettings.LoadSttings();

            string dbFile = "config/dbsettings.txt";
            string ftpFile = "config/ftpsettings.txt";

            if (File.Exists(dbFile) && File.Exists(ftpFile))
            {
                try
                {
                    clsSettings.LoadSttings();

                    // Always set UI fields safely, even if clsSettings properties are null
                    txtServer.Text = clsSettings.DBserver ?? string.Empty;
                    txtUserName.Text = clsSettings.DBuserid ?? string.Empty;
                    txtPwd.Text = clsSettings.DBpwd ?? string.Empty;
                    txtDatabase.Text = clsSettings.DBdatabase ?? string.Empty;
                    checkBox1.Checked = clsSettings.IntegratedSecurity;
                    txtStoreID.Text = clsSettings.StoreID ?? string.Empty;
                    txtFTPpwd.Text = clsSettings.FTPpwd ?? string.Empty;
                    txtFTPserver.Text = clsSettings.FTPserver ?? string.Empty;
                    txtFTPuid.Text = clsSettings.FTPuserid ?? string.Empty;
                    txtUPFolder.Text = clsSettings.FTPupfolder ?? string.Empty;
                    txtBeerTax.Text = clsSettings.Tax ?? string.Empty;
                    textMarkUp.Text = clsSettings.MarkUpPrice.ToString();
                    textDeposit.Text = clsSettings.Deposit.ToString();
                    chkStoked.Checked = clsSettings.stockeditems != 0;
                    chkStoked.Checked = clsSettings.stockeditems == 0 ? false : true;
                    comboBox1.SelectedItem = clsSettings.LastPOSName;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            //   clsSettings.POSList = comboBox1.Items.Cast<string>().ToList();
            if (!string.IsNullOrEmpty(clsSettings.LastPOSName))
            {
                comboBox1.SelectedItem = clsSettings.LastPOSName;
                clsSettings.LastPOSIndex = comboBox1.SelectedIndex;
               
            }
            else if (string.IsNullOrEmpty(clsSettings.LastPOSName))
            {
                comboBox1.SelectedItem = null  ;
            }

            if (comboBox1.SelectedItem == null)
            {
                button2.Enabled = false;
            }

       
            

            // First-run: if files don't exist, skip loading and leave fields empty
            

            // Load categories if file exists
            if (File.Exists(catsFile))
            {
                loadCats();
            }
        }
        private void HeaderCheckBox_MouseClick(object sender, MouseEventArgs e)
        {
            HeaderCheckBoxClick((CheckBox)sender);
        }
        private void loadCats()
        {
            clsQuery query = new clsQuery();
            string jsoncats;
            var fileStream = new FileStream(catsFile, FileMode.Open, FileAccess.Read);

            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                jsoncats = streamReader.ReadToEnd();
            }
            clsCategories[] clscat = JsonConvert.DeserializeObject<clsCategories[]>(jsoncats);
            SqlConnection con;
            try
             {
                dtCat = new System.Data.DataTable();
                dataGridView1.AutoGenerateColumns = false;
                string servername = System.Environment.MachineName.ToString();
                string connectionstring = clsSettings.connectionString;
                con = new SqlConnection(connectionstring);
                SqlCommand cmd;
                if (clsSettings.pcat.Equals("N/A"))
                    cmd = new SqlCommand("", con);
                else
                {
                    cmd = new SqlCommand(" Select Distinct 0 as sel, 0 as dep, " + clsSettings.pcat + " as catname from  " + clsSettings.TableName + " ", con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dtCat);
                    if (clscat != null)
                    {
                        foreach (var itm in clscat)
                        {
                            if (itm.catname == null) itm.catname = "";
                            if (itm.catname.Contains("'"))
                                itm.catname = itm.catname.Replace("'", "''").ToString();
                            DataRow row = dtCat.Select("catname='" + itm.catname.ToString() + "'").FirstOrDefault();
                            if (row != null)
                            {
                                row["sel"] = itm.sel;
                                row["dep"] = itm.dep == 1 ? 1 : 0;
                            }

                        }
                    }
                }

                dataGridView1.ColumnCount = 3;
                dataGridView1.Columns[0].Name = "sel";
                dataGridView1.Columns[0].HeaderText = "Select";
                dataGridView1.Columns[0].DataPropertyName = "sel";

                dataGridView1.Columns[1].Name = "dep";
                dataGridView1.Columns[1].HeaderText = "Deposit";
                dataGridView1.Columns[1].DataPropertyName = "dep";
                 //  dataGridView1.Columns[2].Width = 400;

                dataGridView1.Columns[2].Name = "catname";
                dataGridView1.Columns[2].HeaderText = "Category";
                dataGridView1.Columns[2].DataPropertyName = "catname";
                dataGridView1.Columns[2].Width = 400;

                dataGridView1.DataSource = dtCat.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        CheckBox HeaderCheckBox = null;
        bool IsHeaderCheckBoxClicked = false;
        private void AddHeaderCheckBox()
        {
            HeaderCheckBox = new CheckBox();
            HeaderCheckBox.Size = new Size(15, 15);
            this.dataGridView1.Controls.Add(HeaderCheckBox);
        }
        private void HeaderCheckBoxClick(CheckBox HCheckBox)
        {
            IsHeaderCheckBoxClicked = true;
            foreach (DataGridViewRow dgvr in dataGridView1.Rows)
            {
                ((DataGridViewCheckBoxCell)dgvr.Cells["Sel"]).Value = HCheckBox.Checked;
                dataGridView1.RefreshEdit();
                IsHeaderCheckBoxClicked = false;
            }
        }

        private void btnDbSave_Click(object sender, EventArgs e)
        {


            bool val = false;
            if (checkBox1.Checked)
            {

                if (string.IsNullOrWhiteSpace(txtServer.Text) || string.IsNullOrWhiteSpace(txtDatabase.Text))
                {
                    MessageBox.Show("server and Database fields are mandatory !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    val = true;
                    return;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtServer.Text) || string.IsNullOrWhiteSpace(txtUserName.Text) ||
                    string.IsNullOrWhiteSpace(txtPwd.Text) || string.IsNullOrWhiteSpace(txtDatabase.Text))
                {
                    MessageBox.Show("All fields are mandatory !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    val = true;
                    return;
                }
            }

            if (!val)
            {
                clsDBsettings clsdb = new clsDBsettings
                {
                    server = txtServer.Text,
                    userid = txtUserName.Text,
                    pwd = txtPwd.Text,
                    database = txtDatabase.Text,
                    IntegratedSecurity = checkBox1.Checked
                };

                var serializer = new Newtonsoft.Json.JsonSerializer();
                string dbFile = "config/dbsettings.txt"; // shared file
                using (var sw = new StreamWriter(dbFile))
                using (var writer = new Newtonsoft.Json.JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, clsdb);
                }
                MessageBox.Show("Saved Successfully !!", "Database Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            clsSettings.LoadSttings();

        }

        private void saveFTPsettings_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStoreID.Text) || string.IsNullOrWhiteSpace(txtFTPserver.Text) ||
                    string.IsNullOrWhiteSpace(txtFTPuid.Text) || string.IsNullOrWhiteSpace(txtFTPpwd.Text) ||
                    string.IsNullOrWhiteSpace(txtUPFolder.Text) || string.IsNullOrWhiteSpace(txtBeerTax.Text))
            {
                MessageBox.Show("All fields are mandatory !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("Please select pos_Name in Select POS tab", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                clsFTPsettings clsftp = new clsFTPsettings
                {
                    StoreID = txtStoreID.Text,
                    server = txtFTPserver.Text,
                    userid = txtFTPuid.Text,
                    pwd = txtFTPpwd.Text,
                    upfolder = txtUPFolder.Text,
                    Tax = txtBeerTax.Text,
                    stockeditems = (int)chkStoked.CheckState,
                    MarkUpPrice = string.IsNullOrEmpty(textMarkUp.Text) ? 0 : Convert.ToDecimal(textMarkUp.Text),

                    Deposit = string.IsNullOrEmpty(textDeposit.Text) ? 0 : Convert.ToDecimal(textDeposit.Text),
                    POSName = comboBox1.SelectedItem.ToString(),
                };

                var serializer = new Newtonsoft.Json.JsonSerializer();
                string ftpFile = "config/ftpsettings.txt"; // shared file
                using (var sw = new StreamWriter(ftpFile))
                using (var writer = new Newtonsoft.Json.JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, clsftp);
                }
                if (saveftppop)
                {
                    MessageBox.Show("Saved Successfully !!", "FTP Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                saveftppop = true ; 
                clsSettings.LoadSttings();
            }

        }

        private void btnCatsave_Click(object sender, EventArgs e)
        {
            /*            var query = from r in dtCat.AsEnumerable()
                                        //where r.Field<Int32>("sel") == 1 
                                    select new { sel = r["sel"], dep = r["dep"], catname = r["catname"] };*/

            var query = from r in dtCat.AsEnumerable()
                        select new
                        {
                            sel = r.IsNull("sel") ? 0 : Convert.ToInt32(r["sel"]),
                            dep = r.IsNull("dep") ? 0 : Convert.ToInt32(r["dep"]),
                            catname = r.Field<string>("catname")
                        };

            if (query.Count() == 0)
            {
                MessageBox.Show(" Select Categories ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return ;
            }
            else
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();

                using (StreamWriter sw = new StreamWriter(@"config\cats.txt"))
                using (Newtonsoft.Json.JsonTextWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, query);

                    sw.Close();
                    writer.Close();
                }
            }
            MessageBox.Show("Saved Successfully !!", "Categories", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            clsSettings.LoadSttings();
            this.Close();
        }

        // label for displaying 
        private void label13_Click(object sender, EventArgs e)
        {

        }


        // dropdown for pos selection
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


            if (comboBox1.SelectedIndex >= 0 && !string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                button2.Enabled = true;  // Enable table button
                button1.Enabled = true;  // Enable save button

                clsSettings.LastPOSName = comboBox1.Text;
                clsSettings.LastPOSIndex = comboBox1.SelectedIndex;

                clsSettings.LoadSttings();
            }
            else
            {
                button2.Enabled = false; // Disable table button
                button1.Enabled = false; //Disable save button
            }
            var tableForm = new TableForm();
            saveftppop = false;
            saveFTPsettings_Click(sender, e); 

        }

        // no nedded
        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        // Table button in sleect pos tab
        private void button2_Click(object sender, EventArgs e)
        {


            using (var tableForm = new TableForm())
            {

                tableForm.ShowDialog(); 
            }
            

        }

        // save button in selectpos tab
        private void button1_Click(object sender, EventArgs e)
        {
            saveFTPsettings_Click(sender, e);
        }
        // windows authentication checkbox
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtUserName.Enabled = false;
                txtPwd.Enabled = false;
            }
            else
            {
                txtUserName.Enabled = true;
                txtPwd.Enabled = true;
            }
        }
    }
}

