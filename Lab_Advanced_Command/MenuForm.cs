using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_Advanced_Command
{
    public partial class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponent();
        }

        private void btnFoodForm_Click(object sender, EventArgs e)
        {
            // Mở Form Hướng dẫn (FoodForm)
            FoodForm form = new FoodForm();
            form.Show();
        }

        private void btnOrdersForm_Click(object sender, EventArgs e)
        {
            // Mở Bài tập 2 (OrdersForm)
            OrdersForm form = new OrdersForm();
            form.Show();
        }

        private void btnAccountForm_Click(object sender, EventArgs e)
        {
            // Mở Bài tập 3 (AccountForm)
            AccountForm form = new AccountForm();
            form.Show();
        }

        private void btnTableForm_Click(object sender, EventArgs e)
        {
            // Mở Bài tập 4 (TableForm)
            TableForm form = new TableForm();
            form.Show();
        }
    }
}