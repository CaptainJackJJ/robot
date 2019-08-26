namespace exam
{
    partial class Form_exam
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
            this.button_evaluate = new System.Windows.Forms.Button();
            this.textBox_id = new System.Windows.Forms.TextBox();
            this.textBox_email = new System.Windows.Forms.TextBox();
            this.richTextBox_answer = new System.Windows.Forms.RichTextBox();
            this.textBox_result = new System.Windows.Forms.TextBox();
            this.richTextBox_detail = new System.Windows.Forms.RichTextBox();
            this.radioButton_top_class3 = new System.Windows.Forms.RadioButton();
            this.radioButton_top_class2 = new System.Windows.Forms.RadioButton();
            this.radioButton_vip_class3 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // button_evaluate
            // 
            this.button_evaluate.Location = new System.Drawing.Point(166, 173);
            this.button_evaluate.Name = "button_evaluate";
            this.button_evaluate.Size = new System.Drawing.Size(235, 52);
            this.button_evaluate.TabIndex = 0;
            this.button_evaluate.Text = "evaluate";
            this.button_evaluate.UseVisualStyleBackColor = true;
            this.button_evaluate.Click += new System.EventHandler(this.button_evaluate_Click);
            // 
            // textBox_id
            // 
            this.textBox_id.Location = new System.Drawing.Point(38, 26);
            this.textBox_id.Name = "textBox_id";
            this.textBox_id.Size = new System.Drawing.Size(195, 21);
            this.textBox_id.TabIndex = 1;
            // 
            // textBox_email
            // 
            this.textBox_email.Location = new System.Drawing.Point(391, 26);
            this.textBox_email.Name = "textBox_email";
            this.textBox_email.Size = new System.Drawing.Size(189, 21);
            this.textBox_email.TabIndex = 1;
            // 
            // richTextBox_answer
            // 
            this.richTextBox_answer.Location = new System.Drawing.Point(38, 53);
            this.richTextBox_answer.Name = "richTextBox_answer";
            this.richTextBox_answer.Size = new System.Drawing.Size(542, 110);
            this.richTextBox_answer.TabIndex = 2;
            this.richTextBox_answer.Text = "";
            // 
            // textBox_result
            // 
            this.textBox_result.Location = new System.Drawing.Point(38, 242);
            this.textBox_result.Name = "textBox_result";
            this.textBox_result.Size = new System.Drawing.Size(542, 21);
            this.textBox_result.TabIndex = 1;
            // 
            // richTextBox_detail
            // 
            this.richTextBox_detail.Location = new System.Drawing.Point(38, 291);
            this.richTextBox_detail.Name = "richTextBox_detail";
            this.richTextBox_detail.Size = new System.Drawing.Size(542, 110);
            this.richTextBox_detail.TabIndex = 2;
            this.richTextBox_detail.Text = "";
            // 
            // radioButton_top_class3
            // 
            this.radioButton_top_class3.AutoSize = true;
            this.radioButton_top_class3.Checked = true;
            this.radioButton_top_class3.Location = new System.Drawing.Point(38, 4);
            this.radioButton_top_class3.Name = "radioButton_top_class3";
            this.radioButton_top_class3.Size = new System.Drawing.Size(83, 16);
            this.radioButton_top_class3.TabIndex = 3;
            this.radioButton_top_class3.TabStop = true;
            this.radioButton_top_class3.Text = "top_class3";
            this.radioButton_top_class3.UseVisualStyleBackColor = true;
            // 
            // radioButton_top_class2
            // 
            this.radioButton_top_class2.AutoSize = true;
            this.radioButton_top_class2.Location = new System.Drawing.Point(166, 4);
            this.radioButton_top_class2.Name = "radioButton_top_class2";
            this.radioButton_top_class2.Size = new System.Drawing.Size(83, 16);
            this.radioButton_top_class2.TabIndex = 3;
            this.radioButton_top_class2.Text = "top_class2";
            this.radioButton_top_class2.UseVisualStyleBackColor = true;
            // 
            // radioButton_vip_class3
            // 
            this.radioButton_vip_class3.AutoSize = true;
            this.radioButton_vip_class3.Location = new System.Drawing.Point(301, 4);
            this.radioButton_vip_class3.Name = "radioButton_vip_class3";
            this.radioButton_vip_class3.Size = new System.Drawing.Size(83, 16);
            this.radioButton_vip_class3.TabIndex = 3;
            this.radioButton_vip_class3.Text = "vip_class3";
            this.radioButton_vip_class3.UseVisualStyleBackColor = true;
            // 
            // Form_exam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 432);
            this.Controls.Add(this.radioButton_vip_class3);
            this.Controls.Add(this.radioButton_top_class2);
            this.Controls.Add(this.radioButton_top_class3);
            this.Controls.Add(this.richTextBox_detail);
            this.Controls.Add(this.richTextBox_answer);
            this.Controls.Add(this.textBox_email);
            this.Controls.Add(this.textBox_result);
            this.Controls.Add(this.textBox_id);
            this.Controls.Add(this.button_evaluate);
            this.Name = "Form_exam";
            this.Text = "exam";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_evaluate;
        private System.Windows.Forms.TextBox textBox_id;
        private System.Windows.Forms.TextBox textBox_email;
        private System.Windows.Forms.RichTextBox richTextBox_answer;
        private System.Windows.Forms.TextBox textBox_result;
        private System.Windows.Forms.RichTextBox richTextBox_detail;
        private System.Windows.Forms.RadioButton radioButton_top_class3;
        private System.Windows.Forms.RadioButton radioButton_top_class2;
        private System.Windows.Forms.RadioButton radioButton_vip_class3;
    }
}

