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
    public partial class AddCategoryForm : Form
    {
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";

        // Lớp nội bộ để hiển thị chữ (Đồ ăn) nhưng lưu số (1)
        private class CategoryType
        {
            public int Value { get; set; }
            public string Display { get; set; }
        }

        public AddCategoryForm()
        {
            InitializeComponent();
        }

        private void AddCategoryForm_Load(object sender, EventArgs e)
        {
            // Tải dữ liệu cho ComboBox Type
            List<CategoryType> list = new List<CategoryType>();
            list.Add(new CategoryType() { Value = 1, Display = "Đồ ăn" });
            list.Add(new CategoryType() { Value = 0, Display = "Thức uống" });

            cbbType.DataSource = list;
            cbbType.DisplayMember = "Display";
            cbbType.ValueMember = "Value";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Tên nhóm không được để trống.");
                return;
            }

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            // Dùng SP "Category_InsertUpdateDelete" đã có trong CSDL
            SqlCommand sqlCommand = new SqlCommand("Category_InsertUpdateDelete", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output; // Nhận ID trả về
            sqlCommand.Parameters.AddWithValue("@Name", txtName.Text);
            sqlCommand.Parameters.AddWithValue("@Type", cbbType.SelectedValue);
            sqlCommand.Parameters.AddWithValue("@Action", 0); // 0 = Thêm

            sqlConnection.Open();
            int numAffected = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            if (numAffected > 0)
            {
                MessageBox.Show("Thêm nhóm món ăn thành công.");
                this.Close(); // Đóng Form sau khi thêm
            }
            else
            {
                MessageBox.Show("Thêm thất bại.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}