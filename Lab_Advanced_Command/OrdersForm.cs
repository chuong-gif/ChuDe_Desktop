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
    public partial class OrdersForm : Form
    {
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";

        public OrdersForm()
        {
            InitializeComponent();

            // Đặt ngày mặc định
            dtpFromDate.Value = DateTime.Now.AddDays(-30); // 30 ngày trước
            dtpToDate.Value = DateTime.Now;
        }

        private void btnViewBills_Click(object sender, EventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            // Dùng SP "GetBillsByDateRange" đã có trong CSDL
            SqlCommand sqlCommand = new SqlCommand("GetBillsByDateRange", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            // Lấy ngày tháng từ DateTimePicker
            DateTime fromDate = dtpFromDate.Value.Date; // Lấy 00:00:00
            DateTime toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // Lấy 23:59:59

            // Gán tham số cho SP
            sqlCommand.Parameters.AddWithValue("@FromDate", fromDate);
            sqlCommand.Parameters.AddWithValue("@ToDate", toDate);

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            DataTable dtBills = new DataTable();

            sqlConnection.Open();
            sqlDataAdapter.Fill(dtBills);
            sqlConnection.Close();

            // Hiển thị danh sách hóa đơn
            dgvBills.DataSource = dtBills;

            // Tính toán và hiển thị tổng
            decimal totalAmount = 0;
            decimal totalDiscount = 0;
            decimal totalRevenue = 0;

            foreach (DataRow row in dtBills.Rows)
            {
                totalAmount += Convert.ToDecimal(row["Amount"]);
                totalDiscount += Convert.ToDecimal(row["Discount"]);

                // Tính Thực thu = Tổng - Giảm giá + Thuế
                totalRevenue += Convert.ToDecimal(row["Amount"]) -
                                Convert.ToDecimal(row["Discount"]) +
                                Convert.ToDecimal(row["Tax"]);
            }

            lblTotalAmount.Text = totalAmount.ToString("N0") + " đ";
            lblTotalDiscount.Text = totalDiscount.ToString("N0") + " đ";
            lblTotalRevenue.Text = totalRevenue.ToString("N0") + " đ";
        }

        private void dgvBills_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Đảm bảo người dùng nhấp vào một dòng hợp lệ
            if (e.RowIndex >= 0)
            {
                // Lấy ID của hóa đơn từ cột "ID"
                int billID = Convert.ToInt32(dgvBills.Rows[e.RowIndex].Cells["ID"].Value);

                // Tạo Form chi tiết
                OrderDetailsForm detailsForm = new OrderDetailsForm();

                // Gọi hàm load dữ liệu
                detailsForm.LoadBillDetails(billID);

                // Hiển thị Form
                detailsForm.ShowDialog(this);
            }
        }
    }
}