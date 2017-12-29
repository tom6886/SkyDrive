using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkyDrive.Models
{
    public class RequestResult
    {
        public int Flag { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
    }
}
