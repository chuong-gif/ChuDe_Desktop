using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; // Thư viện quan trọng để làm việc với SQL Server

namespace Lab_Basic_Command
{
    public partial class Form1 : Form
    {
        // Biến chuỗi kết nối (dùng tên server MSI của bạn)
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";

        public Form1()
        {
            InitializeComponent();
        }

        // --- PHẦN 1: LẤY VÀ HIỂN THỊ DANH SÁCH (DÙNG SQLDATAREADER) ---
        private void btnLoad_Click(object sender, EventArgs e)
        {
            // 1. Tạo kết nối
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            // 2. Tạo lệnh
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            // 3. Câu truy vấn
              sqlCommand.CommandText = "SELECT ID, Name, Type FROM Category"; 

            // 4. Mở kết nối
            sqlConnection.Open();

            // 5. Thực thi và lấy dữ liệu
              SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(); 

            // 6. Hiển thị dữ liệu
              this.DisplayCategory(sqlDataReader); 

            // 7. Đóng kết nối
            sqlConnection.Close();
        }

        private void DisplayCategory(SqlDataReader reader)
        {
            // Xóa hết dữ liệu cũ
              lvCategory.Items.Clear(); 

            // Đọc từng dòng dữ liệu
              while (reader.Read()) 
            {
                // Tạo một dòng mới cho ListView
                  ListViewItem item = new ListViewItem(reader["ID"].ToString()); 

                // Thêm các cột phụ
                  item.SubItems.Add(reader["Name"].ToString()); 
                  item.SubItems.Add(reader["Type"].ToString()); 

                // Thêm dòng vào ListView
                  lvCategory.Items.Add(item); 
            }
        }

        // --- PHẦN 2, 3, 4: THÊM, SỬA, XÓA (DÙNG EXECUTE NON-QUERY VÀ PARAMETERS) ---

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtType.Text))
            {
                MessageBox.Show("Tên nhóm và Loại không được để trống!");
                return;
            }

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            // SỬ DỤNG PARAMETERS (Tham số) - Đây là kỹ thuật của Chủ đề 5, rất an toàn
              sqlCommand.CommandText = "INSERT INTO Category (Name, [Type]) VALUES (@Name, @Type)"; 

            // Thêm tham số
            sqlCommand.Parameters.AddWithValue("@Name", txtName.Text);
            sqlCommand.Parameters.AddWithValue("@Type", txtType.Text);

            sqlConnection.Open();

            // Thực thi lệnh (ExecuteNonQuery dùng cho INSERT, UPDATE, DELETE)
              int numOfRowsAffected = sqlCommand.ExecuteNonQuery(); 

            sqlConnection.Close();

            if (numOfRowsAffected == 1)
            {
                  MessageBox.Show("Thêm nhóm món ăn thành công"); 
                
                // Tải lại danh sách
                  btnLoad.PerformClick(); 

                // Xóa các ô nhập
                  txtName.Text = ""; 
                  txtType.Text = ""; 
            }
            else
            {
                  MessageBox.Show("Đã có lỗi xảy ra. Vui lòng thử lại");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            // SỬ DỤNG PARAMETERS
              sqlCommand.CommandText = "UPDATE Category SET Name = @Name, [Type] = @Type WHERE ID = @ID"; 
            
            // Thêm tham số
            sqlCommand.Parameters.AddWithValue("@ID", txtID.Text);
            sqlCommand.Parameters.AddWithValue("@Name", txtName.Text);
            sqlCommand.Parameters.AddWithValue("@Type", txtType.Text);

            sqlConnection.Open();
            int numOfRowsAffected = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            if (numOfRowsAffected == 1)
            {
                // Cập nhật lại dòng trên ListView
                ListViewItem item = lvCategory.SelectedItems[0];
                item.SubItems[1].Text = txtName.Text;
                item.SubItems[2].Text = txtType.Text;

                // Xóa các ô nhập
                txtID.Text = "";
                txtName.Text = "";
                txtType.Text = "";

                // Tắt nút
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;

                MessageBox.Show("Cập nhật nhóm món ăn thành công");
            }
            else
            {
                MessageBox.Show("Đã có lỗi xảy ra. Vui lòng thử lại");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa nhóm này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            // SỬ DỤNG PARAMETERS
              sqlCommand.CommandText = "DELETE FROM Category WHERE ID = @ID"; 

            // Thêm tham số
            sqlCommand.Parameters.AddWithValue("@ID", txtID.Text);

            sqlConnection.Open();
            int numOfRowsAffected = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            if (numOfRowsAffected == 1)
            {
                // Xóa dòng khỏi ListView
                lvCategory.Items.Remove(lvCategory.SelectedItems[0]);

                // Xóa các ô nhập
                txtID.Text = "";
                txtName.Text = "";
                txtType.Text = "";

                // Tắt nút
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;

                MessageBox.Show("Xóa nhóm món ăn thành công");
            }
            else
            {
                MessageBox.Show("Đã có lỗi xảy ra. Vui lòng thử lại");
            }
        }

        // --- PHẦN 5: SỰ KIỆN PHỤ TRỢ ---

        private void lvCategory_Click(object sender, EventArgs e)
        {
            // Lấy dòng được chọn
              ListViewItem item = lvCategory.SelectedItems[0]; 

            // Hiển thị dữ liệu lên Textbox
            txtID.Text = item.Text;
            txtName.Text = item.SubItems[1].Text;
            txtType.Text = item.SubItems[2].Text;

            // Bật nút Cập nhật và Xóa
              btnUpdate.Enabled = true; 
              btnDelete.Enabled = true; 
        }

        private void tsmDelete_Click(object sender, EventArgs e)
        {
            // Gọi sự kiện Click của nút Xóa
            if (lvCategory.SelectedItems.Count > 0)
            {
                  btnDelete.PerformClick(); 
            }
        }

        private void tsmViewFood_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem ID có rỗng không
              if (txtID.Text != "") 
            {
                // Tạo một Form mới
                  FoodForm foodForm = new FoodForm(); 

                // Hiển thị Form đó
                  foodForm.Show(this); 

                // Gọi hàm LoadFood và truyền ID của Category qua
                  foodForm.LoadFood(Convert.ToInt32(txtID.Text)); 
            }
        }

        private void btnViewBills_Click(object sender, EventArgs e)
        {
            BillsForm billsForm = new BillsForm();
            billsForm.Show();
        }

        private void btnAccountManager_Click(object sender, EventArgs e)
        {
            AccountManager accountForm = new AccountManager();
            accountForm.Show();
        }

        private void btnTableManager_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }
    }
}