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
    public partial class RoleAssignmentForm : Form
    {
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";
        string currentAccountName; // Biến lưu tên tài khoản đang xem

        public RoleAssignmentForm()
        {
            InitializeComponent();
        }

        // Hàm này được gọi từ AccountForm
        public void LoadRoles(string accountName)
        {
            currentAccountName = accountName;
            this.Text = "Phân quyền cho tài khoản: " + accountName;
        }

        private void RoleAssignmentForm_Load(object sender, EventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            // 1. Lấy TẤT CẢ các vai trò (dùng SP "Role_GetAll")
            SqlDataAdapter adapterRoles = new SqlDataAdapter("Role_GetAll", sqlConnection);
            adapterRoles.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataTable dtRoles = new DataTable();
            adapterRoles.Fill(dtRoles);

            // Đưa tất cả vai trò vào CheckedListBox
            clbRoles.DataSource = dtRoles;
            clbRoles.DisplayMember = "RoleName"; // Hiển thị cột tên
            clbRoles.ValueMember = "ID"; // Giá trị là cột ID

            // 2. Lấy các vai trò MÀ TÀI KHOẢN NÀY ĐANG CÓ (dùng SP "RoleAccount_GetByAccountName")
            SqlDataAdapter adapterAccountRoles = new SqlDataAdapter("RoleAccount_GetByAccountName", sqlConnection);
            adapterAccountRoles.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapterAccountRoles.SelectCommand.Parameters.AddWithValue("@AccountName", currentAccountName);
            DataTable dtAccountRoles = new DataTable();
            adapterAccountRoles.Fill(dtAccountRoles);

            // 3. Tích chọn vào các vai trò mà tài khoản đó đang có (Actived = 1)
            for (int i = 0; i < clbRoles.Items.Count; i++)
            {
                DataRowView rowView = (DataRowView)clbRoles.Items[i];
                int roleID = (int)rowView["ID"];

                // Tìm xem tài khoản này có vai trò này không
                bool hasRole = dtAccountRoles.AsEnumerable().Any(row => (int)row["RoleID"] == roleID);
                if (hasRole)
                {
                    clbRoles.SetItemChecked(i, true);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            // 1. Xóa tất cả quyền cũ của tài khoản này (dùng SP "RoleAccount_DeleteByAccount")
            SqlCommand cmdDelete = new SqlCommand("RoleAccount_DeleteByAccount", sqlConnection);
            cmdDelete.CommandType = CommandType.StoredProcedure;
            cmdDelete.Parameters.AddWithValue("@AccountName", currentAccountName);
            cmdDelete.ExecuteNonQuery();

            // 2. Thêm lại các quyền được tích chọn (dùng SP "RoleAccount_Insert")
            for (int i = 0; i < clbRoles.CheckedItems.Count; i++)
            {
                DataRowView rowView = (DataRowView)clbRoles.CheckedItems[i];
                int roleID = (int)rowView["ID"];

                SqlCommand cmdInsert = new SqlCommand("RoleAccount_Insert", sqlConnection);
                cmdInsert.CommandType = CommandType.StoredProcedure;
                cmdInsert.Parameters.AddWithValue("@AccountName", currentAccountName);
                cmdInsert.Parameters.AddWithValue("@RoleID", roleID);
                cmdInsert.Parameters.AddWithValue("@Actived", true);
                cmdInsert.Parameters.AddWithValue("@Notes", ""); // Ghi chú (nếu có)
                cmdInsert.ExecuteNonQuery();
            }

            sqlConnection.Close();
            MessageBox.Show("Cập nhật quyền thành công!");
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}