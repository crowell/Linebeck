using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.ComponentModel;
using HundredMilesSoftware.UltraID3Lib;
using System.Threading;

namespace SSLinebeck_wf
{
    public class ThreadedTcpSrvr
    {
        private TcpListener client; //listen for connections
     
        public static int SID = -1; //song id, so we start at 0

        public static List<Song> musicQueue = new List<Song>(); //the queue

        public static Process mplaying = new Process(); //call mplayer here
         
        public static bool isMono = false; //useful for debugging

        public static bool isPlaying = false; //so we don't play more than one song at the same time

        public static void monocheck() //useful for debugging
        {
            Type t = Type.GetType("Mono.Runtime");
            if (t != null)
                isMono = true;
            else
                isMono = false;
        }

        public bool ByteArrayToFile(string _FileName, byte[] _ByteArray) //write the song to a file
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

        public void BackgroundWorkerExecProcess(Process process) //execute mplayer on new thread
        {
            isPlaying = true;
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = false;
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += WorkerCompleted;
            worker.RunWorkerAsync(process);
        }
        void DoWork(object sender, DoWorkEventArgs e) //mplayer dowork
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Process process = e.Argument as Process;
            process.Start();
            process.WaitForExit();
            isPlaying = false;
        }
        void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        } //I don't know why this is needed

        public int GetSID() //return and increment the SID
        {
            return ++SID;
        }

        public string GetSongInfo(string song, int SID) //extract ID3 tags
        {
            string toReturn = "\n" + "SID " + SID + "\n";
            UltraID3 id3 = new UltraID3();
            id3.Read(song);
            toReturn += "Title: " + id3.Title + "\n";
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
                    if (isPlaying == false)
                    {
                        Program.gui.updateNP(String.Empty);
                    }
                    if (musicQueue.Count > 0 && isPlaying == false)
                    {
                        isPlaying = true;
                        //we'll play it
                        if(System.IO.File.Exists("currentsong.media"))
                        {
                            System.IO.File.Delete("currentsong.media");
                        }
                        Song thisSong = musicQueue[0];
                        string currentSong = thisSong.song;
                        if(System.IO.File.Exists(thisSong.song))
                        {
                            System.IO.File.Move(thisSong.song, "currentsong.media");
                        }
                        string currentPath = System.IO.Path.GetFullPath(currentSong);
                        if (System.IO.File.Exists("mplayer.exe"))
                        {
                            mplaying.StartInfo.FileName = "mplayer.exe";
                        }
                        else
                        {
                            mplaying.StartInfo.FileName = "mplayer";
                        }

                        mplaying.StartInfo.Arguments = " " + "currentsong.media" + " ";
                        mplaying.StartInfo.UseShellExecute = false;
                        mplaying.StartInfo.CreateNoWindow = true;
                        //mplaying.StartInfo.RedirectStandardError = true;
                        //mplaying.StartInfo.RedirectStandardOutput = true;
                        BackgroundWorkerExecProcess(mplaying);
                        //System.Console.WriteLine("now playing " + thisSong.info);
                        Program.gui.updateNP(thisSong.info.Replace('\n', '\t'));
                        //deleting file previous
                        musicQueue.RemoveAt(0);
                    }
                    String queue = String.Empty;
                    foreach (Song song in musicQueue)
                    {
                        queue += song.info.Replace('\n', '\t') + "\n";
                    }
                    Program.gui.updateQueue(queue);
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
        } //does EVERYTHING, I should modularize this
    }
}
