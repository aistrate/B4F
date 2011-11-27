namespace B4F.TotalGiro.Jobs.Console
{
    partial class JobConsole
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnGetData = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txtTable = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStartJob = new System.Windows.Forms.Button();
            this.txtStartJob = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(657, 13);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(657, 43);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnGetData
            // 
            this.btnGetData.Location = new System.Drawing.Point(657, 73);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(75, 23);
            this.btnGetData.TabIndex = 2;
            this.btnGetData.Text = "GetData";
            this.btnGetData.UseVisualStyleBackColor = true;
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(618, 423);
            this.dataGridView1.TabIndex = 3;
            // 
            // txtTable
            // 
            this.txtTable.Location = new System.Drawing.Point(657, 119);
            this.txtTable.Name = "txtTable";
            this.txtTable.Size = new System.Drawing.Size(21, 20);
            this.txtTable.TabIndex = 4;
            this.txtTable.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(657, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "table";
            // 
            // btnStartJob
            // 
            this.btnStartJob.Location = new System.Drawing.Point(657, 167);
            this.btnStartJob.Name = "btnStartJob";
            this.btnStartJob.Size = new System.Drawing.Size(75, 23);
            this.btnStartJob.TabIndex = 6;
            this.btnStartJob.Text = "Start Job";
            this.btnStartJob.UseVisualStyleBackColor = true;
            this.btnStartJob.Click += new System.EventHandler(this.btnStartJob_Click);
            // 
            // txtStartJob
            // 
            this.txtStartJob.Location = new System.Drawing.Point(657, 196);
            this.txtStartJob.Name = "txtStartJob";
            this.txtStartJob.Size = new System.Drawing.Size(75, 20);
            this.txtStartJob.TabIndex = 7;
            // 
            // JobConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(757, 521);
            this.Controls.Add(this.txtStartJob);
            this.Controls.Add(this.btnStartJob);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTable);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnGetData);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Name = "JobConsole";
            this.Text = "JobConsole";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnGetData;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox txtTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStartJob;
        private System.Windows.Forms.TextBox txtStartJob;
    }
}