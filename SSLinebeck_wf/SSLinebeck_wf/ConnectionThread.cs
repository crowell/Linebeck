using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SSLinebeck_wf
{
    class ConnectionThread
    {
        public byte[] song;
        public TcpListener threadListener;
        private static int connections = 0;
        public int readsize = 1024;

        public void HandleConnection()
        {
            int recv;
            byte[] data = new byte[1024];

            TcpClient client = threadListener.AcceptTcpClient();
            NetworkStream ns = client.GetStream();
            connections++;
            //        Console.WriteLine("New client accepted: {0} active connections", connections);

            string welcome = "Give me a song! (or a command if you'd like)";
            data = Encoding.ASCII.GetBytes(welcome);
            ns.Write(data, 0, data.Length);



            while (true)
            {
                //  data = new byte[90000000]; //enough bytes for a good buffer
                byte[] length = new byte[12];
                recv = 0;
                bool getlength = false;
                int tempint = 0;
                tempint += ns.Read(length, 0, 12);
                if (tempint > 0)
                {
                    getlength = true;
                }
                if (getlength == true)
                {
                    string len = System.Text.ASCIIEncoding.GetEncoding(1251).GetString(length);
                    Int32.TryParse(len, out tempint);
                    if (tempint < 1024)
                    {
                        data = new byte[tempint];
                        readsize = tempint;
                    }
                    else
                    {
                        data = new byte[tempint + 1024];
                    }
                }


                for (int ii = 0; ii < data.Length; ii++)
                {

                    if (getlength)
                    {
                        recv += ns.Read(data, recv, readsize);
                    }
                    if (recv == tempint)
                    {
                        break;
                    }
                }

                string gotit = "got " + recv + " bytes from you!\n";
                byte[] confirm = Encoding.ASCII.GetBytes(gotit);
                ns.Write(confirm, 0, confirm.Length);


                if (getlength)
                {
                    if (data.Length < 1024 + 25)
                    {
                        song = null;
                        string command = System.Text.ASCIIEncoding.GetEncoding(1251).GetString(data);
                        if (command == "gcq")
                        {
                            string toWrite = String.Empty;
                            int count = ThreadedTcpSrvr.musicQueue.Count;
                            for (int ii = 0; ii < count; ii++)
                            {
                                // toWrite += "SID:" + ThreadedTcpSrvr.musicQueue[ii].SID;
                                toWrite += "\n" + ThreadedTcpSrvr.musicQueue[ii].info;
                                toWrite += "\n";
                            }
                            if (count == 0)
                            {
                                toWrite = "nothing in the queue";
                            }
                            byte[] sendQueue = Encoding.ASCII.GetBytes(toWrite);
                            ns.Write(sendQueue, 0, sendQueue.Length);
                            ns.Close();
                            client.Close();
                            connections--;
                            break;
                        }
                        if (command == "kill current")
                        {
                            if (ThreadedTcpSrvr.isPlaying)
                            {
                                ThreadedTcpSrvr.mplaying.Kill();
                            }
                            ns.Close();
                            client.Close();
                            connections--;
                            break;
                        }
                        string[] twoparts = command.Split(' ');
                        if (twoparts.Length > 1)
                        {
                            if (twoparts[0] == "rm")
                            {
                                int tokill;
                                bool worked = Int32.TryParse(twoparts[1], out tokill);
                                if (worked)
                                {
                                    for (int jj = 0; jj < (ThreadedTcpSrvr.musicQueue.Count); jj++)
                                    {
                                        if (ThreadedTcpSrvr.musicQueue[jj].SID == tokill)
                                        {
                                            if(System.IO.File.Exists(ThreadedTcpSrvr.musicQueue[jj].song))
                                            {
                                                System.IO.File.Delete(ThreadedTcpSrvr.musicQueue[jj].song);
                                            }
                                            ThreadedTcpSrvr.musicQueue.RemoveAt(jj);
                                        }
                                    }
                                    ns.Close();
                                    client.Close();
                                    connections--;
                                    break;
                                }
                            }
                            ns.Close();
                            client.Close();
                            connections--;
                            break;
                        }


                    }
                    else
                    {
                        song = data;
                        break;
                    }
                }
            }
            ns.Close();
            client.Close();
            connections--;
            // Console.WriteLine("Client disconnected: {0} active connections", connections);
        }
    }
}