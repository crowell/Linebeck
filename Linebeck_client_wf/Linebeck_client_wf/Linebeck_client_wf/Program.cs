using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace Linebeck_client_wf
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    public class SimpleTcpClient
    {

        public static string GetFixedLengthString(string input, int length)
        {
            input = input ?? string.Empty;
            input = input.Length > length ? input.Substring(0, length) : input;
            return string.Format("{0,-" + length + "}", input);
        }
        public static byte[] GetBytesFromFile(string fullFilePath) //loads song to byte array
        {
            FileStream fs = File.OpenRead(fullFilePath);
            try
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                return bytes;
            }
            finally
            {
                fs.Close();
            }
        }

        public static string go(string ip, string fileinput)
        {
            string returning = string.Empty;
            byte[] data = new byte[1024];
            string input, stringData;
            IPEndPoint ipep = new IPEndPoint(
                            IPAddress.Parse(ip), 8009);

            Socket server = new Socket(AddressFamily.InterNetwork,
                           SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.Connect(ipep);
            }
            catch (SocketException e)
            {
                 returning += ("Unable to connect to server.");
                Console.WriteLine(e.ToString());
                return returning;
            }


            int recv = server.Receive(data);
            stringData = Encoding.ASCII.GetString(data, 0, recv);
            returning += (stringData);
            {
                input = fileinput;
                if (!File.Exists(input)) // so, we can send commands, if not a song
                {
                    server.Send(Encoding.ASCII.GetBytes(GetFixedLengthString(Convert.ToString(input.Length), 12)));
                    server.Send(Encoding.ASCII.GetBytes(input));
                }
                else
                {
                    byte[] fileToSend = GetBytesFromFile(input);

                    string filelength = GetFixedLengthString(Convert.ToString(fileToSend.Length), 12);

                    server.Send(Encoding.ASCII.GetBytes(filelength));

                    server.Send(fileToSend);
                }
                data = new byte[1024];
                recv = server.Receive(data);
                stringData = Encoding.ASCII.GetString(data, 0, recv);
                returning += (stringData);
                if (input == "gcq")
                {
                    data = new byte[1000000];
                    recv = server.Receive(data);
                    stringData = Encoding.ASCII.GetString(data, 0, recv);
                    string[] queue = stringData.Split('\r');
                    foreach (string str in queue)
                    {
                        returning += str;
                    }
                }

            }

            returning += ("Disconnecting from server...");
            server.Shutdown(SocketShutdown.Both);
            server.Close();
            //Console.ReadKey();
            return returning;
        }
    }
}
