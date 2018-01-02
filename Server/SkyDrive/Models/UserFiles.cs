using MongoDB.Bson;
using System;

namespace SkyDrive.Models
{
    public class UserFiles
    {
        public ObjectId Id { get; set; }

        /// <summary>
        /// 对应保存的实体文件ID
        /// </summary>
        public ObjectId FileID { get; set; }

        public string FileName { get; set; }

        public string MD5 { get; set; }

        public string UserID { get; set; }

        public DateTime UploadTime { get; set; }
    }
}
