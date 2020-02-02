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

        string m_log_pre = "top5-";
        private void gen_log_pre()
        {
            if (radioButton_top_class5.Checked)
            {
                m_log_pre = "top5-";
            }
            else if (radioButton_top_class4.Checked)
            {
                m_log_pre = "top4-";
            }
            else if (radioButton_top_class3.Checked)
            {
                m_log_pre = "top3-";
            }
            else if (radioButton_top_class2.Checked)
            {
                m_log_pre = "top2-";
            }
            else if (radioButton_vip_class3.Checked)
            {
                m_log_pre = "vip3-";
            }
        }

        private void add_pass_log(string msg)
        {
            
            WriteLine(m_log_pre + "pass", msg);
        }

        private void add_not_pass_log(string msg)
        {
            WriteLine(m_log_pre + "not_pass", msg);
        }

        bool IsNumeric(char c) 
        {
            byte bt = Convert.ToByte(c);
            ASCIIEncoding ascii = new ASCIIEncoding();

            if (bt >= 48 && bt <= 57)                          //0~9数字对应十进制48－57
            {
                return true;
            }

            return false;                                      
        }

        bool IsAlphabet(char c)
        {
            byte bt = Convert.ToByte(c);
            ASCIIEncoding ascii = new ASCIIEncoding();

            if (bt >= 97 && bt <= 122)                          //a~z字母对应的十进制97－122
            {
                return true;
            }

            if (bt >= 65 && bt <= 90)                          //A~Z字母对应的十进制65－90
            {
                return true;
            }

            return false;
        }

        bool IsComma(char c)
        {
            if (c == ',')
                return true;
            return false;
        }

        private string check_comma(string answer)
        {
            string new_answer = "";
            char pre_c = ' ';
            foreach (char c in answer)
            {
                if (IsNumeric(c))
                {
                    if(IsAlphabet(pre_c))
                    {
                        new_answer += ',';
                    }
                }
                else if (IsAlphabet(c))
                {
                    if (IsNumeric(pre_c))
                    {
                        new_answer += ',';
                    }
                }
                else if (IsComma(c))
                {
                    if (IsComma(c))
                    {
                        continue;
                    }
                }
                new_answer += c;
                pre_c = c;
            }
            return new_answer;
        }

        private void evaluate()
        {
            gen_log_pre();
            
            //answer_top_class5
            string[] answer_true = { "a","abc","abcd","a","a","a","abcd","ab","abc","abc","b","b","b","a","abc","a","bc","abc","abc","abd","c","c","abcd","acd","bc"};

            if (radioButton_top_class4.Checked)
            {
                string[] answer_temp = { "abc", "a", "a", "a", "abc", "abd", "acd", "b", "abc", "ac", "ab", "abcd", "ab", "ab", "ab", "ab", "abc" };
                answer_true = answer_temp;
            }
            else if(radioButton_top_class3.Checked)
            {
                string[] answer_temp = { "abc", "a", "c", "b", "ac", "abc", "b", "c", "a", "abc", "abc", "a", "a", "b", "b", "bc", "abd"
                                             , "a", "b", "b", "b"};
                answer_true = answer_temp;
            }
            else if (radioButton_top_class2.Checked)
            {
                string[] answer_temp = { "b", "d", "c", "a", "c", "abd", "b", "c", "b", "b", "bc", "ab", "c", "b", "ab", "a", "a", "a", "abcd", "a" };
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
            richTextBox_answer.Text = check_comma(richTextBox_answer.Text); 
            string[] answer = remove_answer_index(richTextBox_answer.Text.Split(','));

            List<int> wrong_answer_index = new List<int>();
            for (int i = 0; i < answer.Length; i++)
            {
                if(answer[i] != answer_true[i])
                {
                    wrong_answer_index.Add(i);
                }
            }


            if ((!radioButton_vip_class3.Checked && wrong_answer_index.Count <= 1) ||
               (radioButton_vip_class3.Checked && wrong_answer_index.Count <= 3))
            {
                textBox_result.Text = textBox_id.Text + "：pass";
                add_pass_log(textBox_id.Text);
                add_pass_log(textBox_email.Text);
                add_pass_log(" ");
            }
            else if ((!radioButton_vip_class3.Checked && wrong_answer_index.Count > 1) ||
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
