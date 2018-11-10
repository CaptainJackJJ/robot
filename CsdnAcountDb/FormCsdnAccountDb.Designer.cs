namespace CsdnAcountDb
{
    partial class FormCsdnAccountDb
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
            this.components = new System.ComponentModel.Container();
            this.buttonCreateDB = new System.Windows.Forms.Button();
            this.buttonCreateServerTable = new System.Windows.Forms.Button();
            this.buttonCheckEmail = new System.Windows.Forms.Button();
            this.timer_CheckEmail = new System.Windows.Forms.Timer(this.components);
            this.webBrowser1 = new CsdnAcountDb.EmailBrowser();
            this.SuspendLayout();
            // 
            // buttonCreateDB
            // 
            this.buttonCreateDB.Enabled = false;
            this.buttonCreateDB.Location = new System.Drawing.Point(12, 12);
            this.buttonCreateDB.Name = "buttonCreateDB";
            this.buttonCreateDB.Size = new System.Drawing.Size(76, 23);
            this.buttonCreateDB.TabIndex = 4;
            this.buttonCreateDB.Text = "CreateDB";
            this.buttonCreateDB.UseVisualStyleBackColor = true;
            this.buttonCreateDB.Click += new System.EventHandler(this.buttonCreateDB_Click);
            // 
            // buttonCreateServerTable
            // 
            this.buttonCreateServerTable.Location = new System.Drawing.Point(93, 2);
            this.buttonCreateServerTable.Name = "buttonCreateServerTable";
            this.buttonCreateServerTable.Size = new System.Drawing.Size(126, 50);
            this.buttonCreateServerTable.TabIndex = 5;
            this.buttonCreateServerTable.Text = "CreateServerTable";
            this.buttonCreateServerTable.UseVisualStyleBackColor = true;
            this.buttonCreateServerTable.Click += new System.EventHandler(this.buttonCreateServerTable_Click);
            // 
            // buttonCheckEmail
            // 
            this.buttonCheckEmail.Location = new System.Drawing.Point(256, 2);
            this.buttonCheckEmail.Name = "buttonCheckEmail";
            this.buttonCheckEmail.Size = new System.Drawing.Size(119, 50);
            this.buttonCheckEmail.TabIndex = 3;
            this.buttonCheckEmail.Text = "checkEmail";
            this.buttonCheckEmail.UseVisualStyleBackColor = true;
            this.buttonCheckEmail.Click += new System.EventHandler(this.buttonCheckEmail_Click);
            // 
            // timer_CheckEmail
            // 
            this.timer_CheckEmail.Tick += new System.EventHandler(this.timer_CheckEmail_Tick);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(1255, 489);
            this.webBrowser1.TabIndex = 2;
            // 
            // FormCsdnAccountDb
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1255, 489);
            this.Controls.Add(this.buttonCheckEmail);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.buttonCreateServerTable);
            this.Controls.Add(this.buttonCreateDB);
            this.Name = "FormCsdnAccountDb";
            this.Text = "CsdnAccountDb";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormCsdnAccountDb_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCreateDB;
        private System.Windows.Forms.Button buttonCreateServerTable;
        private System.Windows.Forms.Button buttonCheckEmail;
        private EmailBrowser webBrowser1;
        private System.Windows.Forms.Timer timer_CheckEmail;
    }
}

