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
            this.buttonCreateDB = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCreateDB
            // 
            this.buttonCreateDB.Enabled = false;
            this.buttonCreateDB.Location = new System.Drawing.Point(12, 29);
            this.buttonCreateDB.Name = "buttonCreateDB";
            this.buttonCreateDB.Size = new System.Drawing.Size(75, 23);
            this.buttonCreateDB.TabIndex = 0;
            this.buttonCreateDB.Text = "CreateDB";
            this.buttonCreateDB.UseVisualStyleBackColor = true;
            this.buttonCreateDB.Click += new System.EventHandler(this.buttonCreateDB_Click);
            // 
            // FormCsdnAccountDb
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.buttonCreateDB);
            this.Name = "FormCsdnAccountDb";
            this.Text = "CsdnAccountDb";
            this.Load += new System.EventHandler(this.FormCsdnAccountDb_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCreateDB;
    }
}

