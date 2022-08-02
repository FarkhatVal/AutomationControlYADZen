using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutomationControlYADZen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }

        private void StartButton1Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem =="Создать аккаунты") MessageBox.Show("1");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            //throw new System.NotImplementedException();
        }
    }
}