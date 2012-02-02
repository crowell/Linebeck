using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.ComponentModel; //background working
using System.Text.RegularExpressions;
using HundredMilesSoftware.UltraID3Lib;

namespace SSLinebeck_wf
{
    class Linebeck //launch the server
    {
        public static ThreadedTcpSrvr myServer;
        public static void ServeItUp()
        {
            myServer = new ThreadedTcpSrvr();
        }
    }
    public static class Program
    {
        public static Form1 gui;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            gui = new Form1();
            ThreadStart serving = new ThreadStart(Linebeck.ServeItUp);
            Thread linebeckthread = new Thread(serving);
            linebeckthread.Start();
            Application.Run(gui);            
        }
    }
}