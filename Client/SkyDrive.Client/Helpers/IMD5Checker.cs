using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyDrive.Client
{
    internal class IMD5Checker
    {
        internal string ID { get; set; }

        internal int State { get; set; }

        internal double Progress { get; set; }

        internal MD5Checker MD5Checker { get; set; }

        internal delegate void AsyncCheckHandler(IMD5Checker sender, AsyncCheckEventArgs e);

        internal event AsyncCheckHandler AsyncCheckProgress;

        internal delegate void CheckEndHandler(IMD5Checker sender, string MD5);

        internal event CheckEndHandler CheckEnd;

        internal IMD5Checker(string id)
        {
            ID = id;
            State = 1;
            Progress = 0;
            MD5Checker = new MD5Checker();
            MD5Checker.AsyncCheckProgress += MD5Checker_AsyncCheckProgress;
        }

        private void MD5Checker_AsyncCheckProgress(AsyncCheckEventArgs e)
        {
            AsyncCheckProgress?.Invoke(this, e);
        }
    }
}
