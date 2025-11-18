using Global_MAU.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
namespace Global_MAU
{
    public partial class TableForm : Form
    {
        public TableForm()
        {
            InitializeComponent();
        }

        private void TableSetting_Load(object sender, EventArgs e)
        {
            if (File.Exists("config//Query.txt"))
            {
                clsSettings.LoadSttings();
                txtUpc.Text = clsSettings.upc;
                txtSku.Text = clsSettings.sku;
                txtPName.Text = clsSettings.StoreProductName;
                txtPDesc.Text = clsSettings.StoreDescription;
                txtUom.Text = clsSettings.uom;
                txtPack.Text = clsSettings.pack;
                txtQty.Text = clsSettings.Qty;
                txtPrice.Text = clsSettings.Price;
                txtSprice.Text = clsSettings.sprice;
                txtAltUpc1.Text = clsSettings.altupc1;
                txtAltUpc2.Text = clsSettings.altupc2;
                txtAltUpc3.Text = clsSettings.altupc3;
                txtAltUpc4.Text = clsSettings.altupc4;
                txtAltUpc5.Text = clsSettings.altupc5;
                txtSDate.Text = clsSettings.Start;
                txtLDate.Text = clsSettings.End;
                txtPCat.Text = clsSettings.pcat;
                txtPCat1.Text = clsSettings.pcat1;
                txtPCat2.Text = clsSettings.pcat2;
                txtVintage.Text = clsSettings.vintage;
                txtDiscountable.Text = clsSettings.Discountable;
                txtTable.Text = clsSettings.TableName;
                txtCondition.Text = clsSettings.Condition;
                CheckStaticQTY.Checked = clsSettings.StaticQuantity;
                CheckQTYPack.Checked = clsSettings.QtyPerPack;
                txtCost.Text = clsSettings.cost;
                chkNegtoPos.Checked = clsSettings.NegativeQTY_to_Positive;
                chkRoundupprice.Checked = clsSettings.RoundUpPrice;
                if (CheckStaticQTY.Checked)
                {
                    txtStaticQty.Text = clsSettings.StaticQtyvalue;
                }
                else
                {
                    txtStaticQty.ReadOnly = true;
                }

            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            clsSettings.LoadSttings();
            this.Close();
        }
        public string ErrorMessage(string text)
        {
            MessageBox.Show("Please Enter " + text + " Column");
            return "";
        }
        private void Save_Click(object sender, EventArgs e)
        {

            clsQuery query = new clsQuery();
            if (CheckStaticQTY.Checked)
            {
                query.upc = txtUpc.Text;
            }
            else
            {
                query.upc = string.IsNullOrEmpty(txtUpc.Text) ? ErrorMessage("UPC") : txtUpc.Text;
            }
            query.sku = string.IsNullOrEmpty(txtSku.Text) ? ErrorMessage("SKU") : txtSku.Text;
            query.StoreProductName = string.IsNullOrEmpty(txtPName.Text) ? ErrorMessage("Product Name") : txtPName.Text;
            query.StoreDescription = string.IsNullOrEmpty(txtPDesc.Text) ? ErrorMessage("Product Description") : txtPDesc.Text;
            query.uom = string.IsNullOrEmpty(txtUom.Text) ? "N/A" : txtUom.Text;
            query.pack = string.IsNullOrEmpty(txtPack.Text) ? "N/A" : txtPack.Text;
            query.Qty = string.IsNullOrEmpty(txtQty.Text) ? ErrorMessage("Quantity") : txtQty.Text;
            query.Price = string.IsNullOrEmpty(txtPrice.Text) ? ErrorMessage("Price") : txtPrice.Text;
            query.sprice = string.IsNullOrEmpty(txtSprice.Text) ? "N/A" : txtSprice.Text;
            query.altupc1 = string.IsNullOrEmpty(txtAltUpc1.Text) ? "N/A" : txtAltUpc1.Text;
            query.altupc2 = string.IsNullOrEmpty(txtAltUpc2.Text) ? "N/A" : txtAltUpc2.Text;
            query.altupc3 = string.IsNullOrEmpty(txtAltUpc3.Text) ? "N/A" : txtAltUpc3.Text;
            query.altupc4 = string.IsNullOrEmpty(txtAltUpc4.Text) ? "N/A" : txtAltUpc4.Text;
            query.altupc5 = string.IsNullOrEmpty(txtAltUpc5.Text) ? "N/A" : txtAltUpc5.Text;
            query.Start = string.IsNullOrEmpty(txtSDate.Text) ? "N/A" : txtSDate.Text;
            query.End = string.IsNullOrEmpty(txtLDate.Text) ? "N/A" : txtLDate.Text;
            query.pcat = string.IsNullOrEmpty(txtPCat.Text) ? "N/A" : txtPCat.Text;
            query.pcat1 = string.IsNullOrEmpty(txtPCat1.Text) ? "N/A" : txtPCat1.Text;
            query.pcat2 = string.IsNullOrEmpty(txtPCat2.Text) ? "N/A" : txtPCat2.Text;
            query.vintage = string.IsNullOrEmpty(txtVintage.Text) ? "N/A" : txtVintage.Text;
            query.Discountable = string.IsNullOrEmpty(txtDiscountable.Text) ? "N/A" : txtDiscountable.Text;

            query.cost = string.IsNullOrEmpty(txtCost.Text) ? "N/A" : txtCost.Text ;
            query.TableName = string.IsNullOrEmpty(txtTable.Text) ? ErrorMessage("Table Name") : txtTable.Text ;
            query.Condition = string.IsNullOrEmpty(txtCondition.Text) ?"N/A": txtCondition.Text ;

            query.StaticQuantity = CheckStaticQTY.Checked;
            // query.StaticQtyvalue = query.StaticQuantity ? txtStaticQty.Text: "N/A";
            if (query.StaticQuantity)
            {
                query.StaticQtyvalue = string.IsNullOrWhiteSpace(txtStaticQty.Text) ? ErrorMessage("StaticQuantity") : txtStaticQty.Text;
            }
            else
            {
                query.StaticQtyvalue = "0"; // or empty, depending on your design
            }

            query.QtyPerPack = CheckQTYPack.Checked;
            query.Negative2Positive = chkNegtoPos.Checked;
            query.RoundUpP = chkRoundupprice.Checked;

            string queryFile = "config/query.txt";
            Dictionary<string, clsQuery> allQueries;

            if (File.Exists(queryFile))
            {
                string json = File.ReadAllText(queryFile);
                allQueries = JsonConvert.DeserializeObject<Dictionary<string, clsQuery>>(json)
                             ?? new Dictionary<string, clsQuery>();
            }
            else
            {
                allQueries = new Dictionary<string, clsQuery>();
            }


            string posKey = clsSettings.LastPOSName?.Trim();
            if (!string.IsNullOrWhiteSpace(posKey))
            {
                
                allQueries[posKey] = query;               


                string updatedJson = JsonConvert.SerializeObject(allQueries, Formatting.Indented);
                File.WriteAllText(queryFile, updatedJson);

                MessageBox.Show("Saved Successfully!", "Query saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("POS name is missing. Cannot save query.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            clsSettings.LoadSttings();

        }
        private void CheckStaticQTY_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckStaticQTY.Checked)
            {
                txtStaticQty.Enabled = true;
                txtStaticQty.ReadOnly = false;

                // Load existing value from clsSettings (from query.txt)
                if (!string.IsNullOrWhiteSpace(clsSettings.StaticQtyvalue) && clsSettings.StaticQtyvalue != "N/A")
                {
                    txtStaticQty.Text = clsSettings.StaticQtyvalue;
                }
                else
                {
                    txtStaticQty.Text = "N/A"; // force user to type
                }
            }
            else
            {
                txtStaticQty.Enabled = false;
                txtStaticQty.ReadOnly = true;
                txtStaticQty.Text = "N/A"; // clear when unchecked
            }
            #region old approach
            /*       if (CheckStaticQTY.Checked)
                    {

                        txtStaticQty.Enabled = true;
                        txtStaticQty.ReadOnly = false;
                    }
                    else
                    {
                        txtStaticQty.Enabled = false;
                       txtStaticQty.Text = "";
                        txtStaticQty.ReadOnly = true;
                    }   */
            #endregion
        }

        // QTY/PACK  check box
        private void CheckQTYPack_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
