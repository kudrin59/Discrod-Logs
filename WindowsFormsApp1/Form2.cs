using System;
using System.Windows.Forms;
using static Logs.Form1;

namespace Logs
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Поле пустое!");
                return;
            }
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                if (listBox1.Items[i].ToString() == textBox1.Text)
                {
                    MessageBox.Show("Данная команда уже в списке!");
                    return;
                }
            }
            listBox1.Items.Add(textBox1.Text);
            Com_list.Add(textBox1.Text);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            int num = listBox1.SelectedIndex;
            if (num < 0)
            {
                MessageBox.Show("Укажите команду для удаления!");
                return;
            }
            listBox1.Items.RemoveAt(num);
            Com_list.RemoveAt(num);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < Com_list.Count; i++) listBox1.Items.Add(Com_list[i]);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (work) this.Close();
        }
    }
}