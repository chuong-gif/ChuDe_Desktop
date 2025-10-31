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
    public partial class FoodForm : Form
    {
        // 1. Khai báo các biến này ở cấp độ Form
        // để tất cả các hàm (Load, Save, Delete) đều có thể dùng chung
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";
        SqlDataAdapter sqlDataAdapter;
        DataTable dt;

        public FoodForm()
        {
            InitializeComponent();
        }

        public void LoadFood(int categoryID)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            // Lấy tên Category để làm tiêu đề
            sqlCommand.CommandText = "SELECT Name FROM Category WHERE ID = " + categoryID;
            sqlConnection.Open();
            string catName = sqlCommand.ExecuteScalar().ToString();
            this.Text = "Danh sách các món ăn thuộc nhóm: " + catName;

            // Lấy danh sách Food
            sqlCommand.CommandText = "SELECT * FROM Food WHERE FoodCategoryID = " + categoryID;

            // 2. Khởi tạo SqlDataAdapter với câu lệnh SELECT
            sqlDataAdapter = new SqlDataAdapter(sqlCommand);

            // 3. Tạo một "CommandBuilder"
            // Đây là một "phụ tá" tự động tạo ra các lệnh INSERT, UPDATE, DELETE
            // cho SqlDataAdapter dựa trên câu lệnh SELECT của bạn.
            SqlCommandBuilder builder = new SqlCommandBuilder(sqlDataAdapter);

            // Khởi tạo DataTable và đổ dữ liệu vào
            dt = new DataTable("Food");
            sqlDataAdapter.Fill(dt);

            // Gán DataTable làm nguồn dữ liệu cho DataGridView
            dgvFood.DataSource = dt;

            // Tự động gán FoodCategoryID cho dòng mới
            // Khi bạn thêm dòng mới trên grid, nó sẽ tự điền ID của nhóm này vào
            dgvFood.CellValueChanged += DgvFood_CellValueChanged;
            dgvFood.DefaultValuesNeeded += (sender, e) => e.Row.Cells["colMaNhom"].Value = categoryID;


            sqlConnection.Close();
            sqlConnection.Dispose();
        }

        private void DgvFood_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Hàm này bắt sự kiện khi bạn sửa 1 ô
            // Bạn có thể thêm code kiểm tra dữ liệu ở đây nếu muốn
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 4. Ra lệnh cho Adapter cập nhật CSDL
                // Nó sẽ tự động xem trong 'dt' có dòng nào mới/bị sửa/bị xóa
                // và chạy lệnh SQL tương ứng (INSERT/UPDATE/DELETE).
                sqlDataAdapter.Update(dt);
                MessageBox.Show("Đã lưu thay đổi vào CSDL!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 5. Xử lý nút Delete
            if (dgvFood.SelectedRows.Count == 0)
            {
                MessageBox.Show("Bạn phải chọn một dòng để xóa!");
                return;
            }

            // Lấy dòng đang chọn
            DataGridViewRow selectedRow = dgvFood.SelectedRows[0];

            // Hỏi xác nhận
            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa món ăn này?", "Xác nhận xóa", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Xóa dòng đó ngay trên DataGridView (giao diện)
                    // (Lưu ý: phải chọn cả dòng bằng cách bấm vào ô vuông bên trái)
                    dgvFood.Rows.Remove(selectedRow);

                    // Dùng Adapter để cập nhật thay đổi này xuống CSDL
                    sqlDataAdapter.Update(dt);

                    MessageBox.Show("Đã xóa thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                    // Nếu lỗi, tải lại dữ liệu để khôi phục dòng vừa xóa
                    dt.RejectChanges();
                }
            }
        }
    }
}