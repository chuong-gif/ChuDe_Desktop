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
    public partial class AccountForm : Form
    {
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";

        public AccountForm()
        {
            InitializeComponent();
        }

        private void AccountForm_Load(object sender, EventArgs e)
        {
            // Khi Form được tải, chạy hàm LoadAccounts
            LoadAccounts();
        }

        private void LoadAccounts()
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            // Dùng SP "Account_GetAll" đã có trong CSDL
            SqlCommand sqlCommand = new SqlCommand("Account_GetAll", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

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
            txtTell.Text = "";
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
            txtTell.Text = item.SubItems[3].Text;
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
            // Dùng SP "Account_InsertUpdateDelete"
            SqlCommand sqlCommand = new SqlCommand("Account_InsertUpdateDelete", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@AccountName", txtAccountName.Text);
            sqlCommand.Parameters.AddWithValue("@Password", txtPassword.Text);
            sqlCommand.Parameters.AddWithValue("@FullName", txtFullName.Text);
            sqlCommand.Parameters.AddWithValue("@Email", txtEmail.Text);
            sqlCommand.Parameters.AddWithValue("@Tell", txtTell.Text);
            sqlCommand.Parameters.AddWithValue("@Action", 0); // 0 = Thêm

            sqlConnection.Open();
            try
            {
                int numAffected = sqlCommand.ExecuteNonQuery();
                if (numAffected > 0)
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
            // Dùng SP "Account_InsertUpdateDelete"
            SqlCommand sqlCommand = new SqlCommand("Account_InsertUpdateDelete", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@AccountName", txtAccountName.Text);
            // SP yêu cầu @Password, nhưng chúng ta không cập nhật nó ở đây
            // Gửi mật khẩu rỗng (hoặc mật khẩu cũ nếu có)
            sqlCommand.Parameters.AddWithValue("@Password", DBNull.Value); // Sửa: SP của bạn yêu cầu MK
            sqlCommand.Parameters.AddWithValue("@FullName", txtFullName.Text);
            sqlCommand.Parameters.AddWithValue("@Email", txtEmail.Text);
            sqlCommand.Parameters.AddWithValue("@Tell", txtTell.Text);
            sqlCommand.Parameters.AddWithValue("@Action", 1); // 1 = Sửa

            // Sửa: SP "Account_InsertUpdateDelete" của bạn cập nhật cả Mật khẩu
            // Chúng ta nên dùng SP "UpdateAccount" (cũng có trong CSDL)
            // nhưng nó cũng cập nhật MK. 
            // Tạm thời, chúng ta sẽ cập nhật cả MK nếu người dùng nhập

            sqlCommand.CommandText = "UPDATE Account SET FullName = @FullName, Email = @Email, Tell = @Tell " +
                                     "WHERE AccountName = @AccountName";
            sqlCommand.CommandType = CommandType.Text; // Đổi lại thành Text

            // Xóa tham số cũ
            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@AccountName", txtAccountName.Text);
            sqlCommand.Parameters.AddWithValue("@FullName", txtFullName.Text);
            sqlCommand.Parameters.AddWithValue("@Email", txtEmail.Text);
            sqlCommand.Parameters.AddWithValue("@Tell", txtTell.Text);


            sqlConnection.Open();
            int numAffected = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            if (numAffected > 0)
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
            // Dùng SP "Account_InsertUpdateDelete"
            SqlCommand sqlCommand = new SqlCommand("Account_InsertUpdateDelete", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@AccountName", txtAccountName.Text);
            sqlCommand.Parameters.AddWithValue("@Password", txtPassword.Text); // Lấy MK mới
            sqlCommand.Parameters.AddWithValue("@FullName", txtFullName.Text); // SP yêu cầu
            sqlCommand.Parameters.AddWithValue("@Email", txtEmail.Text); // SP yêu cầu
            sqlCommand.Parameters.AddWithValue("@Tell", txtTell.Text); // SP yêu cầu
            sqlCommand.Parameters.AddWithValue("@Action", 1); // 1 = Sửa

            sqlConnection.Open();
            int numAffected = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            if (numAffected > 0)
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

            // Dùng SP "Account_DeactivateRoles" chúng ta đã tạo
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("Account_DeactivateRoles", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
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

            // Mở Form RoleAssignmentForm
            RoleAssignmentForm roleForm = new RoleAssignmentForm();
            roleForm.LoadRoles(accountName);
            roleForm.ShowDialog(this);
        }

        private void tsmViewLog_Click(object sender, EventArgs e)
        {
            if (lvAccounts.SelectedItems.Count == 0) return;
            string accountName = lvAccounts.SelectedItems[0].SubItems[0].Text;

            // Mở Form AccountLogForm
            AccountLogForm logForm = new AccountLogForm();
            logForm.LoadAccountLog(accountName);
            logForm.ShowDialog(this);
        }
    }
}