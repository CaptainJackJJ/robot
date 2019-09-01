using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace exam
{
    public partial class Form_exam : Form
    {
        public Form_exam()
        {
            InitializeComponent();
        }

        private bool IsOdd(int n)
        {
            return Convert.ToBoolean(n % 2);
        } 

        private string[] remove_answer_index(string[] answer_with_index)
        {
            string[] answer = new string[answer_with_index.Length / 2];
            int j = 0;
            for (int i = 0; i < answer_with_index.Length; i++)
            {
                if (IsOdd(i))
                {
                    answer[j] = answer_with_index[i];
                    j++;
                }
            }
            return answer;
        }

        private void WriteLine(string fileName, string msg)
        {
            try
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(fileName + ".txt"
                    );
                sw.WriteLine(msg);
                sw.Close();
                sw.Dispose();
            }
            catch
            { }
        }

        private void add_pass_log(string msg)
        {
            WriteLine("pass", msg);
        }

        private void add_not_pass_log(string msg)
        {
            WriteLine("not_pass", msg);
        }

        private void evaluate()
        {
            //answer_top_class3
            string[] answer_true = { "abc", "a", "c", "b", "ac", "abc", "b", "c", "a", "abc", "abc", "a", "a", "b", "b", "bc", "abd"
                                             , "a", "b", "b", "b"};
            
            if(radioButton_top_class2.Checked)
            {
                string [] answer_temp = {"b","d","c","a","c","abd","b","c","b","b","bc","ab","c","b","ab","a","a","a","abcd","a"};
                answer_true = answer_temp;
            }
            else if(radioButton_vip_class3.Checked)
            {
                string[] answer_temp = { "abc", "a", "c", "b", "ac", "abc", "b", "c", "a", "abc", "abc", "a", "a", "b", "b", "bc", "abd"
                                             , "a", "b", "b", "b","bc","a","a","b","abc","bc","ab","c","b","ab","a","a","a","abcd","a"};
                answer_true = answer_temp;
            }

            richTextBox_answer.Text = richTextBox_answer.Text.Replace("\n", ",").Replace(" ","").Replace("\t","").Replace("\r",",");
            richTextBox_answer.Text = richTextBox_answer.Text.Replace("，", ",").Replace(".", ",").Replace("。", ",").Replace("、", ",").ToLower();
            richTextBox_answer.Text = richTextBox_answer.Text.Replace(":", ",").Replace("：", ",").Replace("；", ",").Replace(";", ",");
            string[] answer = remove_answer_index(richTextBox_answer.Text.Split(','));

            List<int> wrong_answer_index = new List<int>();
            for (int i = 0; i < answer.Length; i++)
            {
                if(answer[i] != answer_true[i])
                {
                    wrong_answer_index.Add(i);
                }
            }
            

            if (((radioButton_top_class3.Checked || radioButton_top_class2.Checked) && wrong_answer_index.Count <= 1) ||
               (radioButton_vip_class3.Checked && wrong_answer_index.Count <= 3))
            {
                textBox_result.Text = textBox_id.Text + "：pass";
                add_pass_log("\r\t\r\t" + textBox_id.Text + "\r\t" + textBox_email.Text);
            }
            else if(((radioButton_top_class3.Checked || radioButton_top_class2.Checked) && wrong_answer_index.Count > 1) ||
                (radioButton_vip_class3.Checked && wrong_answer_index.Count > 3))
            {
                string wrong_answer_string = textBox_id.Text;
                string wrong_answer_string_detail = "";
                foreach (int i in wrong_answer_index)
                {
                    wrong_answer_string = wrong_answer_string + "," + (i + 1).ToString();
                    wrong_answer_string_detail = wrong_answer_string_detail + (i + 1).ToString() + "," + answer[i] + "," + answer_true[i] + ";";
                }
                textBox_result.Text = wrong_answer_string;
                richTextBox_detail.Text = wrong_answer_string_detail;
                add_not_pass_log(wrong_answer_string);
            }
        }

        private void button_evaluate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_id.Text) || string.IsNullOrEmpty(richTextBox_answer.Text))
            {
                MessageBox.Show("id or answer is empty");
                return;
            }

            textBox_result.Text = richTextBox_detail.Text = "";

            evaluate();
        }
    }
}
