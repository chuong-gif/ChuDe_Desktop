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
    public partial class BillDetailsForm : Form
    {
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";

        public BillDetailsForm()
        {
            InitializeComponent();
        }

        public void LoadBillDetails(int billID)
        {
            // Đặt tên cho Form
            this.Text = "Chi tiết hóa đơn số: " + billID;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            // Viết câu lệnh JOIN 2 bảng BillDetails và Food
            // Tính luôn cột Thành tiền (Total)
            string query = @"SELECT 
                                f.Name, 
                                bd.Quantity, 
                                f.Price, 
                                (bd.Quantity * f.Price) AS Total 
                            FROM BillDetails bd
                            JOIN Food f ON bd.FoodID = f.ID
                            WHERE bd.InvoiceID = @BillID";

            sqlCommand.CommandText = query;

            // Dùng Parameter (Chủ đề 5) để tránh lỗi SQL Injection
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