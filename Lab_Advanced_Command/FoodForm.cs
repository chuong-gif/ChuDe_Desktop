using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient; // Thêm thư viện SQL
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_Advanced_Command
{
    public partial class FoodForm : Form
    {
        // Biến toàn cục để lưu trữ bảng dữ liệu Food
        DataTable foodTable;

        public FoodForm()
        {
            InitializeComponent();
        }

        private void FoodForm_Load(object sender, EventArgs e)
        {
            // Khi Form mở, tải danh sách Category (loại món ăn)
            this.LoadCategory();
        }

        private void LoadCategory()
        {
            string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = conn.CreateCommand();

            // Dùng thủ tục (Stored Procedure) đã có sẵn
            cmd.CommandText = "Category_GetAll";
            cmd.CommandType = CommandType.StoredProcedure; // Báo cho C# biết đây là SP

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            conn.Open();
            adapter.Fill(dt); // Đổ dữ liệu vào bảng
            conn.Close();
            conn.Dispose();

            // Đưa dữ liệu vào ComboBox
            cbbCategory.DataSource = dt;
            cbbCategory.DisplayMember = "Name"; // Hiển thị cột Name
            cbbCategory.ValueMember = "ID"; // Lưu giá trị cột ID
        }

        private void cbbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Sự kiện này xảy ra khi bạn chọn một mục mới trong ComboBox
            if (cbbCategory.SelectedIndex == -1) return; // Bỏ qua nếu chưa chọn

            string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT * FROM Food WHERE FoodCategoryID = @categoryId";

            cmd.Parameters.Add("@categoryId", SqlDbType.Int);

            // --- SỬA LỖI Ở ĐÂY ---
            // (Giống hệt trang 77 trong sách của bạn)
            // Kiểm tra xem SelectedValue có phải là DataRowView không (khi Form mới load)
            if (cbbCategory.SelectedValue is DataRowView)
            {
                // Nếu phải, lấy ID từ BÊN TRONG nó
                DataRowView rowView = cbbCategory.SelectedValue as DataRowView;
                 cmd.Parameters["@categoryId"].Value = rowView["ID"]; 
    }
            else
            {
                // Nếu không (khi người dùng tự bấm), nó đã là Int32 rồi
                 cmd.Parameters["@categoryId"].Value = cbbCategory.SelectedValue;
    }
            // --- KẾT THÚC SỬA LỖI ---

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            foodTable = new DataTable(); // Dùng biến toàn cục

            conn.Open();
            adapter.Fill(foodTable); // Dòng 78 (dòng này sẽ không còn lỗi)
            conn.Close();
            conn.Dispose();

            // Hiển thị dữ liệu lên DataGridView
            dgvFoodList.DataSource = foodTable;

            // Cập nhật 2 label ở dưới cùng
            lblQuantity.Text = foodTable.Rows.Count.ToString();
            lblCatName.Text = cbbCategory.Text;
        }

        // --- CÁC HÀM SAU CHÚNG TA SẼ LÀM Ở BƯỚC TIẾP THEO ---

        private void tsmCalculateQuantity_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem người dùng đã chọn món ăn nào chưa
            if (dgvFoodList.SelectedRows.Count == 0)
            {
                MessageBox.Show("Bạn phải chọn một món ăn để xem số lượng đã bán.");
                return;
            }

            string connectionString = "server=MSI; database=RestaurantManagement; Integrated Security=True";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = conn.CreateCommand();

            // --- ĐÂY LÀ PHẦN QUAN TRỌNG CỦA BÀI ---

            // 2. Viết câu lệnh SELECT với 2 tham số:
            // @foodId: Tham số đầu vào (IN)
            // @numSaleFood: Tham số đầu ra (OUT) để nhận kết quả
            cmd.CommandText = "SELECT @numSaleFood = SUM(Quantity) FROM BilLDetails WHERE FoodID = @foodId";

            // 3. Lấy thông tin món ăn đang được chọn
            DataGridViewRow selectedRow = dgvFoodList.SelectedRows[0];
            DataRowView rowView = selectedRow.DataBoundItem as DataRowView;
            string foodName = rowView["Name"].ToString();
            string unit = rowView["Unit"].ToString();
            int foodId = Convert.ToInt32(rowView["ID"]);

            // 4. "Gắn" giá trị cho các tham số
            cmd.Parameters.Add("@foodId", SqlDbType.Int);
            cmd.Parameters["@foodId"].Value = foodId;

            // 5. Khai báo tham số @numSaleFood là một tham số ĐẦU RA (Output)
            cmd.Parameters.Add("@numSaleFood", SqlDbType.Int);
             cmd.Parameters["@numSaleFood"].Direction = ParameterDirection.Output; 
            // --- KẾT THÚC PHẦN QUAN TRỌNG ---

            conn.Open();

            // 6. Thực thi lệnh
            // Dùng ExecuteNonQuery() vì chúng ta không đọc hàng, chỉ lấy tham số Output
             cmd.ExecuteNonQuery(); 

            // 7. Đọc giá trị từ tham số Output sau khi lệnh chạy xong
             string result = cmd.Parameters["@numSaleFood"].Value.ToString(); 
            if (string.IsNullOrWhiteSpace(result))
            {
                result = "0"; // Nếu món này chưa bán được (SUM = NULL)
            }

            conn.Close();
            conn.Dispose();

            // 8. Hiển thị kết quả
             MessageBox.Show("Tổng số lượng món " + foodName + " đã bán là: " + result + " " + unit); 
        }

        // HÀM NÀY DÙNG ĐỂ TẢI LẠI DANH SÁCH MÓN ĂN SAU KHI THÊM/SỬA
        void foodForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Lấy chỉ mục (index) của category đang được chọn
            int index = cbbCategory.SelectedIndex;

            // Tải lại toàn bộ category
            LoadCategory();

            // Chọn lại category đó
            cbbCategory.SelectedIndex = index;
        }

        private void tsmAddFood_Click(object sender, EventArgs e)
        {
            FoodInfoForm foodForm = new FoodInfoForm();

            // Gán sự kiện FormClosed để tự động tải lại
            foodForm.FormClosed += new FormClosedEventHandler(foodForm_FormClosed);

            foodForm.Show(this); // Hiển thị Form
        }

        private void tsmUpdateFood_Click(object sender, EventArgs e)
        {
            // Lấy thông tin sản phẩm được chọn
            if (dgvFoodList.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvFoodList.SelectedRows[0];
                DataRowView rowView = selectedRow.DataBoundItem as DataRowView;

                FoodInfoForm foodForm = new FoodInfoForm();
                // Gán sự kiện FormClosed
                foodForm.FormClosed += new FormClosedEventHandler(foodForm_FormClosed);

                foodForm.Show(this);

                // Gọi hàm DisplayFoodInfo để đổ dữ liệu lên Form
                foodForm.DisplayFoodInfo(rowView);
            }
        }

        private void txtSearchByName_TextChanged(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem foodTable (bảng dữ liệu) đã có chưa
            // Nếu chưa chọn Category nào, foodTable sẽ là null
            if (foodTable == null) return;

            // 2. Tạo một đối tượng "DataView" (Bộ lọc)
            // DataView là một "lăng kính" cho phép ta xem DataTable theo cách khác
            // mà không làm thay đổi dữ liệu gốc trong foodTable.
            DataView foodView = new DataView(foodTable); 

            // 3. Tạo "biểu thức lọc" (filterExpression)
            // 'Name like %...%' là cú pháp SQL để "Tìm tên có chứa..."
             string filterExpression = "Name LIKE '%" + txtSearchByName.Text + "%'"; 
    
            // 4. Áp dụng bộ lọc cho DataView
            foodView.RowFilter = filterExpression;

            // 5. Gán DataView (đã được lọc) làm nguồn dữ liệu mới cho bảng
             dgvFoodList.DataSource = foodView; 
        }
        // --- BÀI TẬP C - NÚT MỞ FORM ---
        private void btnOrders_Click(object sender, EventArgs e)
        {
            //// Mở Bài tập 2
            //OrdersForm form = new OrdersForm();
            //form.Show();
        }

        private void btnAccounts_Click(object sender, EventArgs e)
        {
            //// Mở Bài tập 3
            //AccountForm form = new AccountForm();
            //form.Show();
        }

        private void btnTables_Click(object sender, EventArgs e)
        {
            //// Mở Bài tập 4
            //TableForm form = new TableForm();
            //form.Show();
        }
    }
}