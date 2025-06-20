namespace Bank_Management_System.Forms.MyScreen
{
    partial class FrmMyScreen
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
            this.btnClose = new System.Windows.Forms.Button();
            this.lblName = new System.Windows.Forms.Label();
            this.pbMyPhoto = new System.Windows.Forms.PictureBox();
            this.btnLinkedin = new System.Windows.Forms.Button();
            this.btnGithub = new System.Windows.Forms.Button();
            this.lblMajor = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbMyPhoto)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.White;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor = System.Drawing.Color.Black;
            this.btnClose.Location = new System.Drawing.Point(26, 302);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(235, 26);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold);
            this.lblName.Location = new System.Drawing.Point(28, 134);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(149, 15);
            this.lblName.TabIndex = 4;
            this.lblName.Text = "Name: Mahdi Moqdad";
            this.lblName.Click += new System.EventHandler(this.lblName_Click);
            // 
            // pbMyPhoto
            // 
            this.pbMyPhoto.Location = new System.Drawing.Point(81, 1);
            this.pbMyPhoto.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pbMyPhoto.Name = "pbMyPhoto";
            this.pbMyPhoto.Size = new System.Drawing.Size(135, 122);
            this.pbMyPhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbMyPhoto.TabIndex = 3;
            this.pbMyPhoto.TabStop = false;
            // 
            // btnLinkedin
            // 
            this.btnLinkedin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLinkedin.FlatAppearance.BorderSize = 0;
            this.btnLinkedin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLinkedin.Location = new System.Drawing.Point(164, 238);
            this.btnLinkedin.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLinkedin.Name = "btnLinkedin";
            this.btnLinkedin.Size = new System.Drawing.Size(65, 51);
            this.btnLinkedin.TabIndex = 1;
            this.btnLinkedin.UseVisualStyleBackColor = true;
            this.btnLinkedin.Click += new System.EventHandler(this.btnLinkedin_Click);
            // 
            // btnGithub
            // 
            this.btnGithub.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGithub.FlatAppearance.BorderSize = 0;
            this.btnGithub.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGithub.Location = new System.Drawing.Point(61, 238);
            this.btnGithub.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnGithub.Name = "btnGithub";
            this.btnGithub.Size = new System.Drawing.Size(65, 51);
            this.btnGithub.TabIndex = 0;
            this.btnGithub.UseVisualStyleBackColor = true;
            this.btnGithub.Click += new System.EventHandler(this.btnGithub_Click);
            // 
            // lblMajor
            // 
            this.lblMajor.AutoSize = true;
            this.lblMajor.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold);
            this.lblMajor.Location = new System.Drawing.Point(28, 162);
            this.lblMajor.Name = "lblMajor";
            this.lblMajor.Size = new System.Drawing.Size(168, 15);
            this.lblMajor.TabIndex = 5;
            this.lblMajor.Text = "Title: Software Developer";
            this.lblMajor.Click += new System.EventHandler(this.lblMajor_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(28, 187);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(235, 30);
            this.label1.TabIndex = 6;
            this.label1.Text = "This project was developed as\r\n a hobby to learn C# and WinForms.";
            // 
            // FrmMyScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(292, 338);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblMajor);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.pbMyPhoto);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnLinkedin);
            this.Controls.Add(this.btnGithub);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FrmMyScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FrmMyScreen";
            this.Load += new System.EventHandler(this.FrmMyScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbMyPhoto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGithub;
        private System.Windows.Forms.Button btnLinkedin;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.PictureBox pbMyPhoto;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblMajor;
        private System.Windows.Forms.Label label1;
    }
}