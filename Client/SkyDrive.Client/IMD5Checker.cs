using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyDrive.Client
{
    internal class IMD5Checker
    {
        internal string ID { get; set; }

        internal int State { get; set; }

        internal MD5Checker MD5Checker { get; set; }

        internal IMD5Checker(string id)
        {
            ID = id;
            State = 0;
            MD5Checker = new MD5Checker();
        }
    }
}
