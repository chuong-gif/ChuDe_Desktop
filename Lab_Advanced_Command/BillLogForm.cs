using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_Advanced_Command
{
    public partial class BillLogForm : Form
    {
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";

        public BillLogForm()
        {
            InitializeComponent();
        }

        public void LoadBillLog(int tableID, string tableName)
        {
            this.Text = "Nhật ký hóa đơn cho bàn: " + tableName;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            // Dùng SP "GetBillLog_ByTableID" (từ CSDL)
            SqlCommand sqlCommand = new SqlCommand("GetBillLog_ByTableID", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@TableID", tableID);

            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dgvBillLog.DataSource = dt;

            // Tính tổng
            int totalBills = dt.Rows.Count;
            decimal totalRevenue = 0;
            foreach (DataRow row in dt.Rows)
            {
                totalRevenue += Convert.ToDecimal(row["Revenue"]);
            }

            lblTotalBills.Text = "Tổng số hóa đơn: " + totalBills;
            lblTotalRevenue.Text = "Tổng doanh thu: " + totalRevenue.ToString("N0") + " đ";
        }
    }
}