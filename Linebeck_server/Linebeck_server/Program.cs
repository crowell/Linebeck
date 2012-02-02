using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.ComponentModel; //background working
using System.Text.RegularExpressions;
using HundredMilesSoftware.UltraID3Lib;


public class Song
{
    public string info;
    public string song;
    public int SID;
    public Song(string _info, string _song, int _SID)
    {
        this.info = _info;
        this.song = _song;
        this.SID = _SID;
    }
    ~Song()
    {
        if (System.IO.File.Exists(this.song))
        {
            System.IO.File.Delete(this.song);
        }
    }
}


public class ThreadedTcpSrvr
{
    private TcpListener client;

    public static int SID = -1;

    public static List<Song> musicQueue = new List<Song>();

    public static Process mplaying = new Process();

    public static bool isMono = false;

    public static bool isPlaying = false; //so we don't play more than one song at the same time

    public static void monocheck()
    {
        Type t = Type.GetType("Mono.Runtime");
        if (t != null)
            isMono = true;
        else
            isMono = false;
    }

    public bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
    {
        try
        {
            // Open file for reading
            System.IO.FileStream _FileStream = new
 System.IO.FileStream(_FileName, System.IO.FileMode.Create,
 System.IO.FileAccess.Write);

            // Writes a block of bytes to this stream using data from a byte array.
            _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

            // close file stream
            _FileStream.Close();

            return true;
        }
        catch (Exception _Exception)
        {
            // Error
            Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
        }

        // error occured, return false
        return false;
    }

    private void BackgroundWorkerExecProcess(Process process)
    {
        isPlaying = true;
        BackgroundWorker worker = new BackgroundWorker();
        worker.WorkerReportsProgress = false;
        worker.DoWork += DoWork;
        worker.RunWorkerCompleted += WorkerCompleted;
        worker.RunWorkerAsync(process);
    }
    void DoWork(object sender, DoWorkEventArgs e)
    {
        BackgroundWorker worker = sender as BackgroundWorker;
        Process process = e.Argument as Process;
        process.Start();
        process.WaitForExit();
        isPlaying = false;
    }
    void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
    }

    public int GetSID()
    {
        return ++SID;
    }

    public string GetSongInfo(string song, int SID)
    {
        string toReturn = "\n" + "SID " + SID + "\n";
        UltraID3 id3 = new UltraID3();
        id3.Read(song);
        toReturn += "Title: " + id3.Title + "\n" ;
        toReturn += "Artist: " + id3.Artist + "\n";
        toReturn += "Album: " + id3.Album;
        return toReturn;
    }


    public ThreadedTcpSrvr()
    {
        //public List<Song> musicQueue = new List<Song>();
        monocheck();
        client = new TcpListener(8009);
        client.Start();
        Console.WriteLine("Waiting for clients...");
        while (true)
        {
            while (!client.Pending())
            {
                if (musicQueue.Count > 0 && isPlaying == false)
                {
                    isPlaying = true;
                    //we'll play it
                    Song thisSong = musicQueue[0];
                    musicQueue.RemoveAt(0);
                    string currentSong = thisSong.song;
                    // ByteArrayToFile("currentsong.media", currentSong);
                    //Process mplaying = new Process();
                    mplaying.StartInfo.FileName = "mplayer";
                    mplaying.StartInfo.Arguments = currentSong;
                    //mplaying.StartInfo.UseShellExecute = false;
                    mplaying.StartInfo.CreateNoWindow = true;
                    //mplaying.StartInfo.RedirectStandardError = true;
                    //mplaying.StartInfo.RedirectStandardOutput = true;
                    BackgroundWorkerExecProcess(mplaying);
                    System.Console.WriteLine("now playing " + thisSong.info);
                    string previous = Convert.ToString(thisSong.SID - 1);
                    //deleting file is taken care of by the destructor
                    GC.Collect();
                }
                Thread.Sleep(1000);
            }

            ConnectionThread newconnection = new ConnectionThread();
            newconnection.threadListener = this.client;
            Thread newthread = new Thread(new ThreadStart(newconnection.HandleConnection));
            newthread.Start();
            while (newthread.IsAlive)
            {
            }
            byte[] mySong = newconnection.song;
            if (mySong != null)
            {
                int mysongSID = GetSID();
                ByteArrayToFile(SID + ".media", mySong);
                string mySongInfo = GetSongInfo(SID + ".media", SID);
                Song song = new Song(mySongInfo, SID + ".media", mysongSID);
                musicQueue.Add(song);
            }
        }
    }

    public static void Main()
    {
        ThreadedTcpSrvr server = new ThreadedTcpSrvr();
    }
}

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