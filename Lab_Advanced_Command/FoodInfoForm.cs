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
    public partial class FoodInfoForm : Form
    {
        string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";

        public FoodInfoForm()
        {
            InitializeComponent();
        }

        private void FoodInfoForm_Load(object sender, EventArgs e)
        {
            // Khi Form được tải, chạy hàm tải danh sách Category
            this.InitValues();
        }

        private void InitValues()
        {
            // Tải danh sách Category vào ComboBox
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Category_GetAll", conn); // Dùng SP
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet(); // Dùng DataSet

            conn.Open();
            adapter.Fill(ds, "Category"); // Đổ vào 1 bảng tên "Category" trong DataSet
            conn.Close();
            conn.Dispose();

            // Gán dữ liệu cho ComboBox
            cbbCatName.DataSource = ds.Tables["Category"];
            cbbCatName.DisplayMember = "Name";
            cbbCatName.ValueMember = "ID";
        }

        private new void ResetText()
        {
            // Hàm này xóa trắng các ô nhập
            txtFoodID.ResetText();
            txtName.ResetText();
            txtNotes.ResetText();
            txtUnit.ResetText();
            cbbCatName.ResetText();
            nudPrice.ResetText();
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            // --- GỌI THỦ TỤC INSERTFOOD ---
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "InsertFood"; // Tên Stored Procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // Thêm các tham số cho thủ tục
                // @id là tham số OUTPUT
                cmd.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;

                // Các tham số INPUT
                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@Unit", txtUnit.Text);
                cmd.Parameters.AddWithValue("@FoodCategoryID", cbbCatName.SelectedValue);
                cmd.Parameters.AddWithValue("@Price", nudPrice.Value);
                cmd.Parameters.AddWithValue("@Notes", txtNotes.Text);

                conn.Open();
                int numRowAffected = cmd.ExecuteNonQuery();

                if (numRowAffected > 0)
                {
                    // Lấy ID vừa được thêm
                    string foodID = cmd.Parameters["@ID"].Value.ToString();
                    MessageBox.Show("Successfully adding new food. Food ID = " + foodID, "Message");
                    this.ResetText();
                }
                else
                {
                    MessageBox.Show("Adding food failed.");
                }

                conn.Close();
                conn.Dispose();
            }
            catch (SqlException exception)
            {
                MessageBox.Show(exception.Message, "SQL Error");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
            }
        }

        // Hàm này được gọi từ FoodForm để đổ dữ liệu vào khi Cập nhật
        public void DisplayFoodInfo(DataRowView rowView)
        {
            try
            {
                txtFoodID.Text = rowView["ID"].ToString();
                txtName.Text = rowView["Name"].ToString();
                txtUnit.Text = rowView["Unit"].ToString();
                txtNotes.Text = rowView["Notes"].ToString();
                nudPrice.Value = Convert.ToDecimal(rowView["Price"]);

                // Chọn đúng Category trong ComboBox
                cbbCatName.SelectedValue = rowView["FoodCategoryID"];
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                this.Close();
            }
        }

        private void btnUpdateFood_Click(object sender, EventArgs e)
        {
            // --- GỌI THỦ TỤC UPDATEFOOD ---
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "UpdateFood"; // Tên Stored Procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // Thêm các tham số INPUT
                cmd.Parameters.AddWithValue("@ID", int.Parse(txtFoodID.Text));
                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@Unit", txtUnit.Text);
                cmd.Parameters.AddWithValue("@FoodCategoryID", cbbCatName.SelectedValue);
                cmd.Parameters.AddWithValue("@Price", nudPrice.Value);
                cmd.Parameters.AddWithValue("@Notes", txtNotes.Text);

                conn.Open();
                int numRowAffected = cmd.ExecuteNonQuery();

                if (numRowAffected > 0)
                {
                    MessageBox.Show("Successfully updating food.", "Message");
                    this.ResetText();
                }
                else
                {
                    MessageBox.Show("Updating food failed.");
                }

                conn.Close();
                conn.Dispose();
            }
            catch (SqlException exception)
            {
                MessageBox.Show(exception.Message, "SQL Error");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Đóng Form
            this.Close();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {// Mở Form AddCategoryForm
            AddCategoryForm addCategoryForm = new AddCategoryForm();

            // Đăng ký sự kiện FormClosed: Khi Form kia đóng, làm mới ComboBox
            addCategoryForm.FormClosed += (s, args) => {
                this.InitValues();
            };

            // Hiển thị Form (ShowDialog để buộc người dùng phải thao tác xong)
            addCategoryForm.ShowDialog(this);
        }
    }
}