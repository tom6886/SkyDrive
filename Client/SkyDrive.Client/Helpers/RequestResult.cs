using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyDrive.Client
{
    public class RequestResult
    {
        public int Flag { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
    }
}
