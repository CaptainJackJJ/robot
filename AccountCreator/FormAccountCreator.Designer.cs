﻿namespace AccountCreator
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
            this.buttonCreateAccount = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.buttonSetAccount = new System.Windows.Forms.Button();
            this.buttonSetDone = new System.Windows.Forms.Button();
            this.buttonBeFan = new System.Windows.Forms.Button();
            this.button_unbind = new System.Windows.Forms.Button();
            this.label_createNum = new System.Windows.Forms.Label();
            this.webBrowser1 = new AccountCreator.AccountCreatorBrowser();
            this.SuspendLayout();
            // 
            // buttonCreateAccount
            // 
            this.buttonCreateAccount.Location = new System.Drawing.Point(697, 3);
            this.buttonCreateAccount.Name = "buttonCreateAccount";
            this.buttonCreateAccount.Size = new System.Drawing.Size(88, 39);
            this.buttonCreateAccount.TabIndex = 2;
            this.buttonCreateAccount.Text = "Create Account";
            this.buttonCreateAccount.UseVisualStyleBackColor = true;
            this.buttonCreateAccount.Visible = false;
            this.buttonCreateAccount.Click += new System.EventHandler(this.buttonCreateAccount_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // buttonSetAccount
            // 
            this.buttonSetAccount.Location = new System.Drawing.Point(108, 3);
            this.buttonSetAccount.Name = "buttonSetAccount";
            this.buttonSetAccount.Size = new System.Drawing.Size(95, 39);
            this.buttonSetAccount.TabIndex = 3;
            this.buttonSetAccount.Text = "Set Account";
            this.buttonSetAccount.UseVisualStyleBackColor = true;
            this.buttonSetAccount.Visible = false;
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
            this.buttonSetDone.Visible = false;
            this.buttonSetDone.Click += new System.EventHandler(this.buttonSetDone_Click);
            // 
            // buttonBeFan
            // 
            this.buttonBeFan.Location = new System.Drawing.Point(812, 3);
            this.buttonBeFan.Name = "buttonBeFan";
            this.buttonBeFan.Size = new System.Drawing.Size(84, 39);
            this.buttonBeFan.TabIndex = 4;
            this.buttonBeFan.Text = "beFan";
            this.buttonBeFan.UseVisualStyleBackColor = true;
            this.buttonBeFan.Visible = false;
            this.buttonBeFan.Click += new System.EventHandler(this.buttonBeFan_Click);
            // 
            // button_unbind
            // 
            this.button_unbind.Location = new System.Drawing.Point(9, 3);
            this.button_unbind.Name = "button_unbind";
            this.button_unbind.Size = new System.Drawing.Size(93, 39);
            this.button_unbind.TabIndex = 5;
            this.button_unbind.Text = "unbind";
            this.button_unbind.UseVisualStyleBackColor = true;
            this.button_unbind.Visible = false;
            this.button_unbind.Click += new System.EventHandler(this.button_unbind_Click);
            // 
            // label_createNum
            // 
            this.label_createNum.AutoSize = true;
            this.label_createNum.Font = new System.Drawing.Font("SimSun", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_createNum.Location = new System.Drawing.Point(658, 3);
            this.label_createNum.Name = "label_createNum";
            this.label_createNum.Size = new System.Drawing.Size(33, 35);
            this.label_createNum.TabIndex = 6;
            this.label_createNum.Text = "0";
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(-3, 48);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(929, 201);
            this.webBrowser1.TabIndex = 0;
            // 
            // FormAccountCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(925, 261);
            this.Controls.Add(this.label_createNum);
            this.Controls.Add(this.button_unbind);
            this.Controls.Add(this.buttonBeFan);
            this.Controls.Add(this.buttonSetDone);
            this.Controls.Add(this.buttonSetAccount);
            this.Controls.Add(this.buttonCreateAccount);
            this.Controls.Add(this.webBrowser1);
            this.Name = "FormAccountCreator";
            this.Text = "AccountCreator";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormAccountCreator_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AccountCreatorBrowser webBrowser1;
        private System.Windows.Forms.Button buttonCreateAccount;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button buttonSetAccount;
        private System.Windows.Forms.Button buttonSetDone;
        private System.Windows.Forms.Button buttonBeFan;
        private System.Windows.Forms.Button button_unbind;
        private System.Windows.Forms.Label label_createNum;
    }
}

