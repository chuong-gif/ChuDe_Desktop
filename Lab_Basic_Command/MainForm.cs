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
    public partial class MainForm : Form
    {
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";

        // Tạo 1 lớp (class) con để lưu Trạng thái cho ComboBox
        // Giúp hiển thị chữ ("Trống") nhưng lưu giá trị là số (0)
        private class TableStatus
        {
            public int Value { get; set; } // Giá trị (0, 1, 2)
            public string Display { get; set; } // Chữ hiển thị
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 1. Tải danh sách các loại Trạng thái vào ComboBox
            LoadStatusComboBox();

            // 2. Tải danh sách bàn
            LoadTables();
        }

        private void LoadStatusComboBox()
        {
            // Tạo danh sách các trạng thái
            List<TableStatus> statusList = new List<TableStatus>();
            statusList.Add(new TableStatus() { Value = 0, Display = "Trống" });
            statusList.Add(new TableStatus() { Value = 1, Display = "Có khách" });
            statusList.Add(new TableStatus() { Value = 2, Display = "Đã đặt" });

            // Gán cho ComboBox
            cbbStatus.DataSource = statusList;
            cbbStatus.DisplayMember = "Display"; // Hiển thị chữ
            cbbStatus.ValueMember = "Value"; // Lưu giá trị số
        }

        private void LoadTables()
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("Table_GetAll", sqlConnection); // Dùng SP đã có
            sqlCommand.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            lvTables.Items.Clear();
            foreach (DataRow row in dt.Rows)
            {
                ListViewItem item = new ListViewItem(row["ID"].ToString());
                item.SubItems.Add(row["Name"].ToString());

                // Chuyển đổi số (0, 1, 2) sang chữ
                int statusValue = Convert.ToInt32(row["Status"]);
                string statusText = "Trống"; // Mặc định
                if (statusValue == 1) statusText = "Có khách";
                else if (statusValue == 2) statusText = "Đã đặt";
                item.SubItems.Add(statusText);

                item.SubItems.Add(row["Capacity"].ToString());

                lvTables.Items.Add(item);
            }

            ClearTextboxes();
        }

        private void ClearTextboxes()
        {
            txtID.Text = "";
            txtName.Text = "";
            txtCapacity.Text = "";
            cbbStatus.SelectedIndex = 0;
            txtID.Enabled = true;
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void lvTables_Click(object sender, EventArgs e)
        {
            if (lvTables.SelectedItems.Count == 0) return;

            ListViewItem item = lvTables.SelectedItems[0];
            txtID.Text = item.SubItems[0].Text;
            txtName.Text = item.SubItems[1].Text;
            txtCapacity.Text = item.SubItems[3].Text;

            // Tìm và chọn đúng trạng thái trong ComboBox
            string statusText = item.SubItems[2].Text; // Lấy chữ (ví dụ: "Có khách")
            foreach (TableStatus status in cbbStatus.Items)
            {
                if (status.Display == statusText)
                {
                    cbbStatus.SelectedItem = status;
                    break;
                }
            }

            txtID.Enabled = false; // Không cho sửa ID (Khóa chính)
            btnAdd.Enabled = false;
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
        }

        // --- CÁC NÚT THÊM, SỬA, XÓA ---
        // Chúng ta dùng SP "Table_InsertUpdateDelete" mà CSDL của bạn đã cung cấp
        // @Action: 0=Thêm, 1=Sửa, 2=Xóa

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Tên bàn không được để trống!");
                return;
            }

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("Table_InsertUpdateDelete", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            // Thêm tham số cho SP
            sqlCommand.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output; // Biến Output để nhận ID trả về
            sqlCommand.Parameters.AddWithValue("@Name", txtName.Text);
            sqlCommand.Parameters.AddWithValue("@Status", cbbStatus.SelectedValue); // Lấy giá trị số (0, 1, 2)
            sqlCommand.Parameters.AddWithValue("@Capacity", string.IsNullOrEmpty(txtCapacity.Text) ? 0 : Convert.ToInt32(txtCapacity.Text));
            sqlCommand.Parameters.AddWithValue("@Action", 0); // 0 = Thêm

            sqlConnection.Open();
            int numAffected = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            if (numAffected > 0)
            {
                string newID = sqlCommand.Parameters["@ID"].Value.ToString();
                MessageBox.Show("Thêm bàn thành công. Mã bàn mới là: " + newID);
                LoadTables(); // Tải lại danh sách
            }
            else
            {
                MessageBox.Show("Thêm bàn thất bại.");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("Table_InsertUpdateDelete", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@ID", Convert.ToInt32(txtID.Text)); // ID đã có
            sqlCommand.Parameters.AddWithValue("@Name", txtName.Text);
            sqlCommand.Parameters.AddWithValue("@Status", cbbStatus.SelectedValue);
            sqlCommand.Parameters.AddWithValue("@Capacity", string.IsNullOrEmpty(txtCapacity.Text) ? 0 : Convert.ToInt32(txtCapacity.Text));
            sqlCommand.Parameters.AddWithValue("@Action", 1); // 1 = Sửa

            sqlConnection.Open();
            int numAffected = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            if (numAffected > 0)
            {
                MessageBox.Show("Cập nhật bàn thành công.");
                LoadTables(); // Tải lại danh sách
            }
            else
            {
                MessageBox.Show("Cập nhật bàn thất bại.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa bàn này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("Table_InsertUpdateDelete", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.Parameters.AddWithValue("@ID", Convert.ToInt32(txtID.Text));
            sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 1000).Value = DBNull.Value;
            sqlCommand.Parameters.Add("@Status", SqlDbType.Int).Value = DBNull.Value;
            sqlCommand.Parameters.Add("@Capacity", SqlDbType.Int).Value = DBNull.Value;
            sqlCommand.Parameters.AddWithValue("@Action", 2); // 2 = Xóa

            sqlConnection.Open();
            try
            {
                int numAffected = sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();

                if (numAffected > 0)
                {
                    MessageBox.Show("Xóa bàn thành công.");
                    LoadTables(); // Tải lại danh sách
                }
                else
                {
                    MessageBox.Show("Xóa bàn thất bại.");
                }
            }
            catch (SqlException ex)
            {
                // Bắt lỗi nếu xóa bàn đang có hóa đơn (vi phạm khóa ngoại)
                MessageBox.Show("Lỗi: Không thể xóa bàn này vì đã có hóa đơn liên quan. " + ex.Message);
                sqlConnection.Close();
            }
        }

        // --- MENU CHUỘT PHẢI ---
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // Kiểm tra xem có đang chọn dòng nào không
            if (lvTables.SelectedItems.Count == 0)
            {
                // Không chọn, vô hiệu hóa tất cả menu
                tsmViewCurrentBill.Enabled = false;
                tsmViewBillHistory.Enabled = false;
                tsmViewBillLog.Enabled = false;
                tsmDeleteTable.Enabled = false;
            }
            else
            {
                // Có chọn
                tsmViewBillHistory.Enabled = true;
                tsmViewBillLog.Enabled = true;
                tsmDeleteTable.Enabled = true;

                // Kiểm tra trạng thái bàn để bật/tắt "Xem hóa đơn hiện tại"
                string statusText = lvTables.SelectedItems[0].SubItems[2].Text;
                if (statusText == "Có khách") // Chỉ bàn "Có khách" mới có HĐ hiện tại
                {
                    tsmViewCurrentBill.Enabled = true;
                }
                else
                {
                    tsmViewCurrentBill.Enabled = false;
                }
            }
        }

        private void tsmViewCurrentBill_Click(object sender, EventArgs e)
        {
            int tableID = Convert.ToInt32(lvTables.SelectedItems[0].SubItems[0].Text);

            // Tìm hóa đơn CHƯA THANH TOÁN (Status=0) của bàn này
            // Dùng SP "Bills_GetUncheckByTableID" đã có sẵn trong CSDL của bạn
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("Bills_GetUncheckByTableID", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@TableID", tableID);

            sqlConnection.Open();
            object result = sqlCommand.ExecuteScalar(); // Lấy 1 giá trị (ID của hóa đơn)
            sqlConnection.Close();

            if (result != null)
            {
                int currentBillID = Convert.ToInt32(result);

                // Mở Form BillDetails (từ Bài 2) và hiển thị
                BillDetailsForm detailsForm = new BillDetailsForm();
                detailsForm.LoadBillDetails(currentBillID);
                detailsForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Không tìm thấy hóa đơn nào đang mở cho bàn này.");
            }
        }

        private void tsmViewBillHistory_Click(object sender, EventArgs e)
        {
            int tableID = Convert.ToInt32(lvTables.SelectedItems[0].SubItems[0].Text);
            string tableName = lvTables.SelectedItems[0].SubItems[1].Text;

            BillHistoryForm historyForm = new BillHistoryForm();
            historyForm.LoadBillHistory(tableID, tableName);
            historyForm.ShowDialog();
        }

        private void tsmViewBillLog_Click(object sender, EventArgs e)
        {
            int tableID = Convert.ToInt32(lvTables.SelectedItems[0].SubItems[0].Text);
            string tableName = lvTables.SelectedItems[0].SubItems[1].Text;

            BillLogForm logForm = new BillLogForm();
            logForm.LoadBillLog(tableID, tableName);
            logForm.ShowDialog();
        }

        private void tsmDeleteTable_Click(object sender, EventArgs e)
        {
            // Gọi sự kiện Click của nút Xóa
            btnDelete.PerformClick();
        }
    }
}