namespace AccountCreator
{
    partial class FormAccountCreator
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
            this.textBoxPhone = new System.Windows.Forms.TextBox();
            this.buttonCreateAccount = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.webBrowser1 = new AccountCreator.AccountCreatorBrowser();
            this.buttonSetAccount = new System.Windows.Forms.Button();
            this.buttonSetDone = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxPhone
            // 
            this.textBoxPhone.Location = new System.Drawing.Point(567, 13);
            this.textBoxPhone.Name = "textBoxPhone";
            this.textBoxPhone.Size = new System.Drawing.Size(100, 21);
            this.textBoxPhone.TabIndex = 1;
            // 
            // buttonCreateAccount
            // 
            this.buttonCreateAccount.Location = new System.Drawing.Point(697, 3);
            this.buttonCreateAccount.Name = "buttonCreateAccount";
            this.buttonCreateAccount.Size = new System.Drawing.Size(88, 39);
            this.buttonCreateAccount.TabIndex = 2;
            this.buttonCreateAccount.Text = "创建账号";
            this.buttonCreateAccount.UseVisualStyleBackColor = true;
            this.buttonCreateAccount.Click += new System.EventHandler(this.buttonCreateAccount_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(-3, 48);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(929, 201);
            this.webBrowser1.TabIndex = 0;
            // 
            // buttonSetAccount
            // 
            this.buttonSetAccount.Location = new System.Drawing.Point(108, 3);
            this.buttonSetAccount.Name = "buttonSetAccount";
            this.buttonSetAccount.Size = new System.Drawing.Size(95, 39);
            this.buttonSetAccount.TabIndex = 3;
            this.buttonSetAccount.Text = "Set Account";
            this.buttonSetAccount.UseVisualStyleBackColor = true;
            this.buttonSetAccount.Click += new System.EventHandler(this.buttonSetAccount_Click);
            // 
            // buttonSetDone
            // 
            this.buttonSetDone.Enabled = false;
            this.buttonSetDone.Location = new System.Drawing.Point(229, 3);
            this.buttonSetDone.Name = "buttonSetDone";
            this.buttonSetDone.Size = new System.Drawing.Size(95, 39);
            this.buttonSetDone.TabIndex = 3;
            this.buttonSetDone.Text = "Set Done";
            this.buttonSetDone.UseVisualStyleBackColor = true;
            this.buttonSetDone.Click += new System.EventHandler(this.buttonSetDone_Click);
            // 
            // FormAccountCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(925, 261);
            this.Controls.Add(this.buttonSetDone);
            this.Controls.Add(this.buttonSetAccount);
            this.Controls.Add(this.buttonCreateAccount);
            this.Controls.Add(this.textBoxPhone);
            this.Controls.Add(this.webBrowser1);
            this.Name = "FormAccountCreator";
            this.Text = "账号创建助手";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormAccountCreator_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AccountCreatorBrowser webBrowser1;
        private System.Windows.Forms.TextBox textBoxPhone;
        private System.Windows.Forms.Button buttonCreateAccount;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button buttonSetAccount;
        private System.Windows.Forms.Button buttonSetDone;
    }
}

