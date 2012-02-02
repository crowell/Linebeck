using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Linebeck_client_wf
{


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void addtext(string str)
        {
            MessageBox.Show( str , "server response",
        MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ip = textBox1.Text.ToString();
            string command = textBox2.Text.ToString();
            addtext(SimpleTcpClient.go(ip, command));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Linebeck Client find a media file";
            fdlg.InitialDirectory = @"c:\";
            fdlg.Filter = "All files (*.*)|*.*|All files (*.*)|*.*";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = fdlg.FileName;
            }


        }
    }
}
