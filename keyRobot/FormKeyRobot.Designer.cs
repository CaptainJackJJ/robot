namespace experiment
{
    partial class FormKeyRobot
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.navigateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.submitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetNeedFinishNumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerRobotBrain = new System.Windows.Forms.Timer(this.components);
            this.button_start = new System.Windows.Forms.Button();
            this.webBrowser1 = new experiment.CsdnBrowser();
            this.button_stop = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navigateToolStripMenuItem,
            this.submitToolStripMenuItem,
            this.accountToolStripMenuItem,
            this.resetNeedFinishNumToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(276, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // navigateToolStripMenuItem
            // 
            this.navigateToolStripMenuItem.Name = "navigateToolStripMenuItem";
            this.navigateToolStripMenuItem.Size = new System.Drawing.Size(69, 21);
            this.navigateToolStripMenuItem.Text = "navigate";
            this.navigateToolStripMenuItem.Click += new System.EventHandler(this.navigateToolStripMenuItem_Click);
            // 
            // submitToolStripMenuItem
            // 
            this.submitToolStripMenuItem.Name = "submitToolStripMenuItem";
            this.submitToolStripMenuItem.Size = new System.Drawing.Size(59, 21);
            this.submitToolStripMenuItem.Text = "submit";
            this.submitToolStripMenuItem.Click += new System.EventHandler(this.submitToolStripMenuItem_Click);
            // 
            // accountToolStripMenuItem
            // 
            this.accountToolStripMenuItem.Name = "accountToolStripMenuItem";
            this.accountToolStripMenuItem.Size = new System.Drawing.Size(65, 21);
            this.accountToolStripMenuItem.Text = "account";
            this.accountToolStripMenuItem.Click += new System.EventHandler(this.accountToolStripMenuItem_Click);
            // 
            // resetNeedFinishNumToolStripMenuItem
            // 
            this.resetNeedFinishNumToolStripMenuItem.Name = "resetNeedFinishNumToolStripMenuItem";
            this.resetNeedFinishNumToolStripMenuItem.Size = new System.Drawing.Size(144, 21);
            this.resetNeedFinishNumToolStripMenuItem.Text = "ResetNeedFinishNum";
            this.resetNeedFinishNumToolStripMenuItem.Click += new System.EventHandler(this.resetNeedFinishNumToolStripMenuItem_Click);
            // 
            // timerRobotBrain
            // 
            this.timerRobotBrain.Enabled = true;
            this.timerRobotBrain.Interval = 1000;
            this.timerRobotBrain.Tick += new System.EventHandler(this.timerRobotBrain_Tick);
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(124, 54);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(89, 50);
            this.button_start.TabIndex = 3;
            this.button_start.Text = "start";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(12, 68);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(51, 73);
            this.webBrowser1.TabIndex = 0;
            // 
            // button_stop
            // 
            this.button_stop.Location = new System.Drawing.Point(124, 142);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(89, 63);
            this.button_stop.TabIndex = 4;
            this.button_stop.Text = "stop";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // FormKeyRobot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 258);
            this.Controls.Add(this.button_stop);
            this.Controls.Add(this.button_start);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormKeyRobot";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormKeyRobot";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CsdnBrowser webBrowser1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem navigateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem submitToolStripMenuItem;
        private System.Windows.Forms.Timer timerRobotBrain;
        private System.Windows.Forms.ToolStripMenuItem accountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetNeedFinishNumToolStripMenuItem;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.Button button_stop;
    }
}

