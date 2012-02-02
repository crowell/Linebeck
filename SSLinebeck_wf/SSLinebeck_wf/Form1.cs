using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSLinebeck_wf
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false; //BREAK ALL OF THE LAWS!!!!
            InitializeComponent();
        }

        private void rtbQueue_TextChanged(object sender, EventArgs e)
        {

        }
        public void updateNP(string nowPlaying) //update the now playing
        {
            tbNowPlaying.Text = nowPlaying;
        }
        public void updateQueue(string queue) //update the queue
        {
            rtbQueue.Text = queue;
        }
    }
}
