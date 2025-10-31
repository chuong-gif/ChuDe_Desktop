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
    public partial class BillsForm : Form
    {
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";

        public BillsForm()
        {
            InitializeComponent();

            // Đặt ngày mặc định
            dtpFromDate.Value = DateTime.Now.AddDays(-7); // 7 ngày trước
            dtpToDate.Value = DateTime.Now;
        }

        private void btnViewBills_Click(object sender, EventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            // Lấy ngày tháng từ DateTimePicker
            DateTime fromDate = dtpFromDate.Value.Date; // Lấy ngày, bỏ qua giờ
            DateTime toDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1); // Lấy đến 23:59:59 của ngày

            // Câu lệnh SELECT hóa đơn
            string query = @"SELECT 
                                ID, Name, TableID, Amount, Discount, Tax, 
                                (Amount - Discount + Tax) AS Revenue, 
                                CheckoutDate, Account 
                            FROM Bills 
                            WHERE Status = 1 AND CheckoutDate BETWEEN @FromDate AND @ToDate";

            sqlCommand.CommandText = query;
            sqlCommand.Parameters.AddWithValue("@FromDate", fromDate);
            sqlCommand.Parameters.AddWithValue("@ToDate", toDate);

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            DataTable dtBills = new DataTable();

            sqlConnection.Open();
            sqlDataAdapter.Fill(dtBills);
            sqlConnection.Close();

            // Hiển thị danh sách hóa đơn
            dgvBills.DataSource = dtBills;
            dgvBills.Columns["ID"].HeaderText = "Mã HĐ";
            dgvBills.Columns["Name"].HeaderText = "Tên HĐ";
            dgvBills.Columns["TableID"].HeaderText = "Số bàn";
            dgvBills.Columns["Amount"].HeaderText = "Tổng tiền";
            dgvBills.Columns["Discount"].HeaderText = "Giảm giá";
            dgvBills.Columns["Tax"].HeaderText = "Thuế";
            dgvBills.Columns["Revenue"].HeaderText = "Thực thu";
            dgvBills.Columns["CheckoutDate"].HeaderText = "Ngày thanh toán";
            dgvBills.Columns["Account"].HeaderText = "Người lập";

            // Tính toán và hiển thị tổng
            decimal totalAmount = 0;
            decimal totalDiscount = 0;
            decimal totalRevenue = 0;

            foreach (DataRow row in dtBills.Rows)
            {
                totalAmount += Convert.ToDecimal(row["Amount"]);
                totalDiscount += Convert.ToDecimal(row["Discount"]);
                totalRevenue += Convert.ToDecimal(row["Revenue"]);
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
                // Lấy ID của hóa đơn
                int billID = Convert.ToInt32(dgvBills.Rows[e.RowIndex].Cells["ID"].Value);

                // Tạo Form chi tiết
                BillDetailsForm detailsForm = new BillDetailsForm();

                // Gọi hàm load dữ liệu
                detailsForm.LoadBillDetails(billID);

                // Hiển thị Form
                detailsForm.ShowDialog();
            }
        }
    }
}