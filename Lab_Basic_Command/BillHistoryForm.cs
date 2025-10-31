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

namespace Lab_Basic_Command
{
    public partial class BillHistoryForm : Form
    {
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";
        private int currentTableID;

        public BillHistoryForm()
        {
            InitializeComponent();
        }

        public void LoadBillHistory(int tableID, string tableName)
        {
            currentTableID = tableID;
            this.Text = "Lịch sử hóa đơn cho bàn: " + tableName;

            // Load danh sách ngày
            LoadBillDates(tableID);
        }

        private void LoadBillDates(int tableID)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("GetBillDates_ByTableID", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure; // Dùng SP

            sqlCommand.Parameters.AddWithValue("@TableID", tableID);

            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            // Gán nguồn dữ liệu cho ListBox
            lsbBillDates.DataSource = dt;
            lsbBillDates.DisplayMember = "BillDate"; // Tên cột trả về từ SP
            lsbBillDates.ValueMember = "BillDate";

            // Xóa chọn mặc định
            lsbBillDates.ClearSelected();
            dgvBillDetails.DataSource = null;
        }

        private void lsbBillDates_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Khi người dùng chọn 1 ngày
            if (lsbBillDates.SelectedValue == null) return;

            DateTime selectedDate = (DateTime)lsbBillDates.SelectedValue;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("GetBillDetails_ByDateAndTableID", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure; // Dùng SP

            sqlCommand.Parameters.AddWithValue("@TableID", currentTableID);
            sqlCommand.Parameters.AddWithValue("@BillDate", selectedDate.Date);

            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dgvBillDetails.DataSource = dt;
        }
    }
}