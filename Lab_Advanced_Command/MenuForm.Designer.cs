namespace Lab_Advanced_Command
{
    partial class MenuForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.btnFoodForm = new System.Windows.Forms.Button();
            this.btnOrdersForm = new System.Windows.Forms.Button();
            this.btnAccountForm = new System.Windows.Forms.Button();
            this.btnTableForm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(34, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(433, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "MENU CHỨC NĂNG (BÀI TẬP CHỦ ĐỀ 5)";
            // 
            // btnFoodForm
            // 
            this.btnFoodForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFoodForm.Location = new System.Drawing.Point(39, 80);
            this.btnFoodForm.Name = "btnFoodForm";
            this.btnFoodForm.Size = new System.Drawing.Size(190, 70);
            this.btnFoodForm.TabIndex = 1;
            this.btnFoodForm.Text = "Quản lý Món ăn (Hướng dẫn)";
            this.btnFoodForm.UseVisualStyleBackColor = true;
            this.btnFoodForm.Click += new System.EventHandler(this.btnFoodForm_Click);
            // 
            // btnOrdersForm
            // 
            this.btnOrdersForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOrdersForm.Location = new System.Drawing.Point(277, 80);
            this.btnOrdersForm.Name = "btnOrdersForm";
            this.btnOrdersForm.Size = new System.Drawing.Size(190, 70);
            this.btnOrdersForm.TabIndex = 2;
            this.btnOrdersForm.Text = "Quản lý Hóa đơn (Bài tập 2)";
            this.btnOrdersForm.UseVisualStyleBackColor = true;
            this.btnOrdersForm.Click += new System.EventHandler(this.btnOrdersForm_Click);
            // 
            // btnAccountForm
            // 
            this.btnAccountForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAccountForm.Location = new System.Drawing.Point(39, 170);
            this.btnAccountForm.Name = "btnAccountForm";
            this.btnAccountForm.Size = new System.Drawing.Size(190, 70);
            this.btnAccountForm.TabIndex = 3;
            this.btnAccountForm.Text = "Quản lý Tài khoản (Bài tập 3)";
            this.btnAccountForm.UseVisualStyleBackColor = true;
            this.btnAccountForm.Click += new System.EventHandler(this.btnAccountForm_Click);
            // 
            // btnTableForm
            // 
            this.btnTableForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTableForm.Location = new System.Drawing.Point(277, 170);
            this.btnTableForm.Name = "btnTableForm";
            this.btnTableForm.Size = new System.Drawing.Size(190, 70);
            this.btnTableForm.TabIndex = 4;
            this.btnTableForm.Text = "Quản lý Bàn (Bài tập 4)";
            this.btnTableForm.UseVisualStyleBackColor = true;
            this.btnTableForm.Click += new System.EventHandler(this.btnTableForm_Click);
            // 
            // MenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 271);
            this.Controls.Add(this.btnTableForm);
            this.Controls.Add(this.btnAccountForm);
            this.Controls.Add(this.btnOrdersForm);
            this.Controls.Add(this.btnFoodForm);
            this.Controls.Add(this.label1);
            this.Name = "MenuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menu Chính";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnFoodForm;
        private System.Windows.Forms.Button btnOrdersForm;
        private System.Windows.Forms.Button btnAccountForm;
        private System.Windows.Forms.Button btnTableForm;
    }
}