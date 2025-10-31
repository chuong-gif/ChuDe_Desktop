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
    public partial class OrderDetailsForm : Form
    {
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";

        public OrderDetailsForm()
        {
            InitializeComponent();
        }

        // Hàm này được gọi từ OrdersForm
        public void LoadBillDetails(int billID)
        {
            this.Text = "Chi tiết hóa đơn số: " + billID;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            // Dùng SP "GetBillDetails" đã có trong CSDL
            SqlCommand sqlCommand = new SqlCommand("GetBillDetails", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@BillID", billID);

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable();

            sqlConnection.Open();
            sqlDataAdapter.Fill(dt); // Đổ dữ liệu vào bảng
            sqlConnection.Close();

            dgvBillDetails.DataSource = dt; // Hiển thị lên DataGridView
        }
    }
}