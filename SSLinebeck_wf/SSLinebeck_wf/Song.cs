using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLinebeck_wf
{
    public class Song
    {
        public string info; //metadata
        public string song; //song file path
        public int SID;     //Song ID
        public Song(string _info, string _song, int _SID)
        {
            this.info = _info;
            this.song = _song;
            this.SID = _SID;
        }
        ~Song() //destroyed by GC, make sure to delete the files.
        {
            if (System.IO.File.Exists(this.song))
            {
                System.IO.File.Delete(this.song);
            }
        }
    }
}
