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
    public partial class AccountLogForm : Form
    {
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";
        private string currentAccountName;

        public AccountLogForm()
        {
            InitializeComponent();
        }

        public void LoadAccountLog(string accountName)
        {
            currentAccountName = accountName;
            this.Text = "Nhật ký hoạt động cho: " + accountName;

            // Tải danh sách ngày
            LoadBillDates(accountName);
        }

        private void LoadBillDates(string accountName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            // Dùng SP "GetBillDatesByAccount" đã có trong CSDL
            SqlCommand sqlCommand = new SqlCommand("GetBillDatesByAccount", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@AccountName", accountName);

            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            // Gán nguồn dữ liệu cho ListBox
            lsbBillDates.DataSource = dt;
            lsbBillDates.DisplayMember = "CheckoutDate"; // Tên cột trả về từ SP
            lsbBillDates.ValueMember = "CheckoutDate";
            lsbBillDates.FormatString = "dd/MM/yyyy HH:mm"; // Định dạng ngày giờ

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
            // Dùng SP "GetBillDetailsByDate" đã có trong CSDL
            SqlCommand sqlCommand = new SqlCommand("GetBillDetailsByDate", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@AccountName", currentAccountName);
            sqlCommand.Parameters.AddWithValue("@CheckoutDate", selectedDate);

            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dgvBillDetails.DataSource = dt;
        }
    }
}