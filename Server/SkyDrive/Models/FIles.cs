using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkyDrive.Models
{
    public class Files
    {
        public ObjectId Id { get; set; }

        public string FileName { get; set; }

        public string MD5 { get; set; }

        public string Url { get; set; }
    }
}
