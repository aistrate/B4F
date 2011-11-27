namespace VierlanderMigration
{
    partial class MainMenu
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
            this.btnImportLatestFiles = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnImportLatestFiles
            // 
            this.btnImportLatestFiles.Location = new System.Drawing.Point(68, 42);
            this.btnImportLatestFiles.Name = "btnImportLatestFiles";
            this.btnImportLatestFiles.Size = new System.Drawing.Size(175, 23);
            this.btnImportLatestFiles.TabIndex = 0;
            this.btnImportLatestFiles.Text = "Import Latest Files";
            this.btnImportLatestFiles.UseVisualStyleBackColor = true;
            this.btnImportLatestFiles.Click += new System.EventHandler(this.btnImportLatestFiles_Click);
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 364);
            this.Controls.Add(this.btnImportLatestFiles);
            this.Name = "MainMenu";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnImportLatestFiles;
    }
}

