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
    public partial class AccountManager : Form
    {
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";

        public AccountManager()
        {
            InitializeComponent();
        }

        private void AccountManager_Load(object sender, EventArgs e)
        {
            // Khi Form được tải, chạy hàm LoadAccounts
            LoadAccounts();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            // Nút Tải lại
            LoadAccounts();
        }

        private void LoadAccounts()
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            // Lấy TẤT CẢ tài khoản
            sqlCommand.CommandText = "SELECT * FROM Account";

            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            lvAccounts.Items.Clear();
            // Hiển thị lên ListView
            foreach (DataRow row in dt.Rows)
            {
                ListViewItem item = new ListViewItem(row["AccountName"].ToString());
                item.SubItems.Add(row["FullName"].ToString());
                item.SubItems.Add(row["Email"].ToString());
                item.SubItems.Add(row["Tell"].ToString());
                item.SubItems.Add(row["DateCreated"].ToString());

                lvAccounts.Items.Add(item);
            }

            // Xóa sạch các ô nhập
            ClearTextboxes();
        }

        private void ClearTextboxes()
        {
            txtAccountName.Text = "";
            txtPassword.Text = "";
            txtFullName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtAccountName.Enabled = true; // Cho phép nhập Tên TK
        }

        private void lvAccounts_Click(object sender, EventArgs e)
        {
            // Khi bấm vào 1 dòng trong ListView
            if (lvAccounts.SelectedItems.Count == 0) return;

            ListViewItem item = lvAccounts.SelectedItems[0];

            // Hiển thị thông tin lên các ô textbox
            txtAccountName.Text = item.SubItems[0].Text;
            txtFullName.Text = item.SubItems[1].Text;
            txtEmail.Text = item.SubItems[2].Text;
            txtPhone.Text = item.SubItems[3].Text;
            txtPassword.Text = ""; // Không hiển thị mật khẩu

            txtAccountName.Enabled = false; // Không cho sửa Tên TK (vì là khóa chính)
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAccountName.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Tên TK, Mật khẩu và Họ tên không được để trống!");
                return;
            }

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            sqlCommand.CommandText = "INSERT INTO Account (AccountName, Password, FullName, Email, Tell) " +
                          "VALUES (@AccountName, @Password, @FullName, @Email, @Tell)";

            sqlCommand.Parameters.AddWithValue("@AccountName", txtAccountName.Text);
            sqlCommand.Parameters.AddWithValue("@Password", txtPassword.Text); // Lưu ý: thực tế cần mã hóa MK
            sqlCommand.Parameters.AddWithValue("@FullName", txtFullName.Text);
            sqlCommand.Parameters.AddWithValue("@Email", txtEmail.Text);
            sqlCommand.Parameters.AddWithValue("@Tell", txtPhone.Text);

            sqlConnection.Open();
            try
            {
                int numAffected = sqlCommand.ExecuteNonQuery();
                if (numAffected == 1)
                {
                    MessageBox.Show("Thêm tài khoản thành công.");
                    LoadAccounts(); // Tải lại danh sách
                }
                else
                {
                    MessageBox.Show("Thêm tài khoản thất bại.");
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Lỗi vi phạm khóa chính (trùng tên TK)
                {
                    MessageBox.Show("Lỗi: Tên tài khoản này đã tồn tại.");
                }
                else
                {
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
                }
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAccountName.Text))
            {
                MessageBox.Show("Bạn phải chọn một tài khoản để cập nhật!");
                return;
            }

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            sqlCommand.CommandText = "UPDATE Account SET FullName = @FullName, Email = @Email, Tell = @Tell " +
                         "WHERE AccountName = @AccountName";

            sqlCommand.Parameters.AddWithValue("@AccountName", txtAccountName.Text);
            sqlCommand.Parameters.AddWithValue("@FullName", txtFullName.Text);
            sqlCommand.Parameters.AddWithValue("@Email", txtEmail.Text);
            sqlCommand.Parameters.AddWithValue("@Tell", txtPhone.Text);

            sqlConnection.Open();
            int numAffected = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            if (numAffected == 1)
            {
                MessageBox.Show("Cập nhật thông tin thành công.");
                LoadAccounts(); // Tải lại danh sách
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại.");
            }
        }

        private void btnResetPass_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAccountName.Text))
            {
                MessageBox.Show("Bạn phải chọn một tài khoản!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Bạn phải nhập mật khẩu mới vào ô Mật khẩu!");
                return;
            }

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            sqlCommand.CommandText = "UPDATE Account SET Password = @Password WHERE AccountName = @AccountName";

            sqlCommand.Parameters.AddWithValue("@AccountName", txtAccountName.Text);
            sqlCommand.Parameters.AddWithValue("@Password", txtPassword.Text); // Lấy mật khẩu mới từ ô MK

            sqlConnection.Open();
            int numAffected = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            if (numAffected == 1)
            {
                MessageBox.Show("Reset mật khẩu thành công.");
                ClearTextboxes();
            }
            else
            {
                MessageBox.Show("Reset mật khẩu thất bại.");
            }
        }

        // --- MENU CHUỘT PHẢI ---
        private void tsmDeleteAccount_Click(object sender, EventArgs e)
        {
            if (lvAccounts.SelectedItems.Count == 0) return;

            string accountName = lvAccounts.SelectedItems[0].SubItems[0].Text;

            if (MessageBox.Show($"Bạn có chắc muốn XÓA (vô hiệu hóa) tài khoản '{accountName}' không? " +
                                "Toàn bộ vai trò của họ sẽ bị tắt.", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            // Bài tập yêu cầu: "toàn bộ vai trò của tài khoản này sẽ bị đánh dấu là không kích hoạt (0)" 
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            sqlCommand.CommandText = "UPDATE RoleAccount SET Actived = 0 WHERE AccountName = @AccountName";
            sqlCommand.Parameters.AddWithValue("@AccountName", accountName);

            sqlConnection.Open();
            int numAffected = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            MessageBox.Show($"Đã vô hiệu hóa {numAffected} vai trò của tài khoản '{accountName}'.");
            LoadAccounts();
        }

        private void tsmViewRoles_Click(object sender, EventArgs e)
        {
            if (lvAccounts.SelectedItems.Count == 0) return;

            string accountName = lvAccounts.SelectedItems[0].SubItems[0].Text;

            // Mở Form RoleForm và truyền tên tài khoản qua
            RoleForm roleForm = new RoleForm();
            roleForm.LoadRoles(accountName);
            roleForm.ShowDialog();
        }
    }
}