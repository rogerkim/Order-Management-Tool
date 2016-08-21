using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;

namespace MangoOrderImporter
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void btn_upload_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Select file";
            fdlg.InitialDirectory = @"c:\";
            fdlg.FileName = txtFileName.Text;
            fdlg.Filter = "Text and CSV Files(*.txt, *.csv)|*.txt;*.csv|Text Files(*.txt)|*.txt|CSV Files(*.csv)|*.csv|All Files(*.*)|*.*";
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = fdlg.FileName;
                import();
                Application.DoEvents();
            }
        }

        private void import()
        {
            System.IO.StreamReader file = new System.IO.StreamReader(txtFileName.Text);
            string[] columnnames = file.ReadLine().Split(',');
            DataTable dt = new DataTable();
            foreach (string c in columnnames)
            {
                dt.Columns.Add(c);
            }
            string newline;
            while ((newline = file.ReadLine()) != null)
            {
                DataRow dr = dt.NewRow();
                string[] values = newline.Split(',');
                for (int i = 0; i < values.Length; i++)
                {
                    dr[i] = values[i];
                }
                dt.Rows.Add(dr);
            }
            file.Close();
            dataGridView.DataSource = dt;
            dataGridView.ReadOnly = false;
            label3.Text = dt.Rows.Count + " orders are imported.";
            if (dt.Rows.Count > 0) btnExport.Enabled = true;
        }


        private void btnJuso_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("http://juso.go.kr/");
            Process.Start(sInfo);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SqlTransaction trans;

            try
            {
                using (SqlConnection con = new SqlConnection(SQL.CONSTRING))
                {
                    // open transaction.
                    con.Open();
                    trans = con.BeginTransaction();

                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        // check order already exist
                        if (IsTrackingNoExist(trans, con, row))
                        {
                            string mangoOrderId = (string)row.Cells["OrderId"].Value;
                            throw new Exception("Mango OrderId " + mangoOrderId + " is already exist.");
                        }

                        // order table
                        SqlCommand insertOrder = new SqlCommand(SQL.INSERT_ORDERS, con, trans);
                        string[] address = Util.SplitAddress(row.Cells["Address"].Value.ToString());
                        insertOrder.Parameters.AddWithValue("@MangoOrderId", row.Cells["MangoOrderId"].Value);
                        insertOrder.Parameters.AddWithValue("@Name", row.Cells["Name"].Value);
                        insertOrder.Parameters.AddWithValue("@Address1", address[0]);
                        insertOrder.Parameters.AddWithValue("@Address2", address[1]);
                        insertOrder.Parameters.AddWithValue("@Address3", address[2]);
                        insertOrder.Parameters.AddWithValue("@Zipcode", row.Cells["ZipCode"].Value);
                        insertOrder.Parameters.AddWithValue("@Mobile", row.Cells["Mobile"].Value);
                        insertOrder.Parameters.AddWithValue("@OrderDate", row.Cells["OrderDate"].Value);
                        insertOrder.Parameters.AddWithValue("@MangoOrderId", row.Cells["OrderDate"].Value);
                        insertOrder.ExecuteNonQuery();

                        // carton table
                        SqlCommand inserCarton = new SqlCommand(SQL.INESRT_CARTON, con, trans);
                        insertOrder.Parameters.AddWithValue("@MangoOrderId", row.Cells["MangoOrderId"].Value);
                        insertOrder.Parameters.AddWithValue("@LineNo", row.Cells["LineNo"].Value);
                        insertOrder.Parameters.AddWithValue("@TrackingNumber", row.Cells["TrackingNumber"].Value);
                        insertOrder.ExecuteNonQuery();
                    }

                    // close transaction.
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Boolean IsTrackingNoExist(SqlTransaction trans, SqlConnection con, DataGridViewRow row)
        {
            SqlCommand checkTrackingNo = new SqlCommand(SQL.CHECK_TRACKING_NO, con, trans);
            checkTrackingNo.Parameters.AddWithValue("@TrackingNumber", row.Cells["TrackingNo"].Value);
            object scalarValue = checkTrackingNo.ExecuteScalar();
            int count = (int)scalarValue;
            return (count > 0) ? true : false;
        }

        private void btnDBCheck_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQL.CONSTRING))
                {
                    con.Open();
                    MessageBox.Show(SQL.CONSTRING, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Connection Error.\n" + SQL.CONSTRING, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
