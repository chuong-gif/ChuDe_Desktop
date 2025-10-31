namespace Lab_Advanced_Command
{
    partial class BillLogForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) { components.Dispose(); }
            base.Dispose(disposing);
        }
        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.dgvBillLog = new System.Windows.Forms.DataGridView();
            this.lblTotalBills = new System.Windows.Forms.Label();
            this.lblTotalRevenue = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBillLog)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvBillLog
            // 
            this.dgvBillLog.AllowUserToAddRows = false;
            this.dgvBillLog.AllowUserToDeleteRows = false;
            this.dgvBillLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBillLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBillLog.Location = new System.Drawing.Point(12, 12);
            this.dgvBillLog.Name = "dgvBillLog";
            this.dgvBillLog.ReadOnly = true;
            this.dgvBillLog.Size = new System.Drawing.Size(776, 397);
            this.dgvBillLog.TabIndex = 0;
            // 
            // lblTotalBills
            // 
            this.lblTotalBills.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalBills.AutoSize = true;
            this.lblTotalBills.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalBills.Location = new System.Drawing.Point(12, 425);
            this.lblTotalBills.Name = "lblTotalBills";
            this.lblTotalBills.Size = new System.Drawing.Size(121, 16);
            this.lblTotalBills.TabIndex = 1;
            this.lblTotalBills.Text = "Tổng số hóa đơn: 0";
            // 
            // lblTotalRevenue
            // 
            this.lblTotalRevenue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalRevenue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalRevenue.Location = new System.Drawing.Point(544, 425);
            this.lblTotalRevenue.Name = "lblTotalRevenue";
            this.lblTotalRevenue.Size = new System.Drawing.Size(244, 16);
            this.lblTotalRevenue.TabIndex = 2;
            this.lblTotalRevenue.Text = "Tổng doanh thu: 0 đ";
            this.lblTotalRevenue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BillLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblTotalRevenue);
            this.Controls.Add(this.lblTotalBills);
            this.Controls.Add(this.dgvBillLog);
            this.Name = "BillLogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nhật ký hóa đơn của bàn";
            ((System.ComponentModel.ISupportInitialize)(this.dgvBillLog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
        private System.Windows.Forms.DataGridView dgvBillLog;
        private System.Windows.Forms.Label lblTotalBills;
        private System.Windows.Forms.Label lblTotalRevenue;
    }
}