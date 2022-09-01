
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PrintBarcodesFromGridview
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        string cs = "server=172.20.100.123;uid=shop;pwd=shop;database=shop_t;";

        MySqlConnection con;
        MySqlCommand cmd;
        MySqlDataAdapter adp;
        DataTable dtable;
        MySqlDataReader rdr;

        private DataTable dt = new DataTable("Products");
        private List<Image> barcodes = new List<Image>();
        private void Form1_Load(object sender, EventArgs e)
        {
            GetData();
        }

        private void GetData()
        {
            try
            {
                con = new MySqlConnection(cs);
                con.Open();
                cmd = new MySqlCommand("Select RTRIM(Product_Code),RTRIM(Product_Name),RTRIM(JanCode1),RTRIM(P_UnitPrice),RTRIM(M_MerchantName) From Product_New Order By Product_Code", con);
                rdr = cmd.ExecuteReader();
                dgvSelect.Rows.Clear();
                while (rdr.Read())
                {
                    dgvSelect.Rows.Add(rdr[0], rdr[1], rdr[2].ToString().Replace(":", ""), rdr[3]);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {           

            if (lvPrint.Items.Count == 0)
            {
                //no item has been selected message
                MessageBox.Show("商品が選択されていません。", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtNoOfStickers.Value <= 0)
            {
                //choose number of stickers message
                MessageBox.Show("部数を選択してください。", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSearchProduct.Focus();
                return;
            }
            if (Convert.ToInt16(txtNoOfStickers.Text) == 0)
            {
                //number of copies must be greater than 0 message
                MessageBox.Show("部数を選択してください。", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSearchProduct.Focus();
                return;
            }
            try
            {
                if (lvPrint.CheckedItems.Count > 0)
                {
                    string Condition = "";
                    string Condition1 = "";
                    string st;
                    int NoOfStickers = Convert.ToInt32(txtNoOfStickers.Value);
                    for (var I = 0; I <= lvPrint.CheckedItems.Count - 1; I++)
                    {
                        Condition += string.Format("'{0}',", lvPrint.CheckedItems[I].Text);
                        Condition1 += string.Format("':{0}',", lvPrint.CheckedItems[I].SubItems[2].Text);
                    }
                    Condition = Condition.Substring(0, Condition.Length - 1);
                    Condition1 = Condition1.Substring(0, Condition1.Length - 1);
                    Cursor = Cursors.WaitCursor;
                    timer1.Enabled = true;
                    con = new MySqlConnection(cs);
                    if (chkShowPrices.Checked)
                    {
                        st = "Select Product_Code As MRP, JanCode1 As Barcode, Product_Name As ProductName, P_UnitPrice As SalesRate From Product_New Where Product_Code In (" + Condition + ") And JanCode1 In (" + Condition1 + ")";
                        con.Open();

                        for (int j = 1; j <= NoOfStickers - 1; j++)
                            st += "Union All Select Product_Code As MRP, JanCode1 As Barcode, Product_Name As ProductName, P_UnitPrice As SalesRate From Product_New Where Product_Code In (" + Condition + ") And JanCode1 In (" + Condition1 + ")";
                        cmd = new MySqlCommand(st, con);
                        adp = new MySqlDataAdapter(cmd);
                        dtable = new DataTable();
                        adp.Fill(dtable);
                        con.Close();

                        if (dtable.Rows.Count > 0)
                        {
                            DataSet ds = new DataSet();
                            ds.Tables.Add(dtable);
                            ds.WriteXmlSchema("BarcodeLabelPrinting1.xml");
                            rptBarcodeLabelPrinting rpt = new rptBarcodeLabelPrinting();
                            rpt.SetDataSource(ds);
                            frmReport frmReport = new frmReport();
                            frmReport.CrystalReportViewer1.ReportSource = rpt;
                            frmReport.ShowDialog();
                            rpt.Close();
                            rpt.Dispose();
                        }
                        else
                        { MessageBox.Show("Products list Empty!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                    else
                    {
                        st = "Select Product_Code As MRP, JanCode1 As Barcode, Product_Name As ProductName From Product_New Where Product_Code In (" + Condition + ") And JanCode1 In (" + Condition1 + ")";
                        con.Open();

                        for (int j = 1; j <= NoOfStickers - 1; j++)
                            st += "Union All Select Product_Code As MRP, JanCode1 As Barcode, Product_Name As ProductName From Product_New Where Product_Code In (" + Condition + ") And JanCode1 In (" + Condition1 + ")";
                        st += " Order By MRP";
                        cmd = new MySqlCommand(st, con);
                        adp = new MySqlDataAdapter(cmd);
                        dtable = new DataTable();
                        adp.Fill(dtable);
                        con.Close();

                        if (dtable.Rows.Count > 0)
                        {
                            DataSet ds = new DataSet();
                            ds.Tables.Add(dtable);
                            ds.WriteXmlSchema("BarcodeLabelPrinting1.xml");
                            rptBarcodeLabelPrinting rpt = new rptBarcodeLabelPrinting();
                            rpt.SetDataSource(ds);
                            frmReport frmReport = new frmReport();
                            frmReport.CrystalReportViewer1.ReportSource = rpt;
                            frmReport.ShowDialog();
                            rpt.Close();
                            rpt.Dispose();
                        }
                        else
                        { MessageBox.Show("Products list empty!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                }
            }
            finally
            { }
        }

        public List<DataGridViewRow> GetCheckedRows(DataGridView dgv, int col)
        {
            List<DataGridViewRow> checkedRows = new List<DataGridViewRow>();
            for (int inx = 0; inx <= dgv.RowCount - 1; inx++)
            {
                dgv[1, inx].Value = null;
                DataGridViewCheckBoxCell @checked = dgv[col, inx] as DataGridViewCheckBoxCell;
                if (Convert.ToBoolean(@checked.EditedFormattedValue))
                    checkedRows.Add(dgv.Rows[inx]);
            }
            return checkedRows;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Cursor = DefaultCursor;
            timer1.Stop();
        }

        private void txtSearchProduct_TextChanged(object sender, EventArgs e)
        {
            try
            {
                con = new MySqlConnection(cs);
                con.Open();
                cmd = new MySqlCommand("Select RTRIM(Product_Code),RTRIM(Product_Name),RTRIM(JanCode1),RTRIM(P_UnitPrice),RTRIM(M_MerchantName) From Product_New Where M_MerchantName Like @SearchString Or Product_Name Like @SearchString Order By Product_Code", con);
                cmd.Parameters.AddWithValue("@SearchString", "%" + txtSearchProduct.Text.Trim() + "%");
                rdr = cmd.ExecuteReader();
                dgvSelect.Rows.Clear(); 
                while (rdr.Read())
                {
                    dgvSelect.Rows.Add(rdr[0], rdr[1], rdr[2].ToString().Replace(":", ""), rdr[3]);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvSelect_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            bool isExists = false;
            foreach (ListViewItem viewItem in lvPrint.Items)
                if (viewItem.Text == dgvSelect.Rows[e.RowIndex].Cells[0].Value.ToString())
                {
                    isExists = true;
                    break;
                }

            if (isExists) { MessageBox.Show("この商品はすでに選択されています。", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                var item = new ListViewItem();
                item.Text = dgvSelect.Rows[e.RowIndex].Cells[0].Value.ToString();
                item.SubItems.Add(dgvSelect.Rows[e.RowIndex].Cells[1].Value.ToString());
                item.SubItems.Add(dgvSelect.Rows[e.RowIndex].Cells[2].Value.ToString());
                item.SubItems.Add(dgvSelect.Rows[e.RowIndex].Cells[3].Value.ToString());
            lvPrint.Items.Add(item);
                for (var i = 0; i <= lvPrint.Items.Count - 1; i++)
                    lvPrint.Items[i].Checked = true;
                con.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtNoOfStickers.Value = 0;
            txtSearchProduct.Text = "";
            lvPrint.Items.Clear();
            dgvSelect.Rows.Clear();
            txtSearchProduct.Focus();
            GetData();
        }

        private void dgvSelect_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void chkShowPrices_CheckedChanged(object sender, EventArgs e)
        {

        }

    }
}
