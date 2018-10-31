namespace supervisor
{
    partial class FormSupervisor
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
            this.timerSupervisor = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerSupervisor
            // 
            this.timerSupervisor.Enabled = true;
            this.timerSupervisor.Interval = 60000;
            this.timerSupervisor.Tick += new System.EventHandler(this.timerSupervisor_Tick);
            // 
            // FormSupervisor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "FormSupervisor";
            this.Text = "监工";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerSupervisor;
    }
}

