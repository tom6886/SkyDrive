using MongoDB.Bson;

namespace SkyDrive.Models
{
    public class Files
    {
        public ObjectId Id { get; set; }

        public string MD5 { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// 已上传文件流的长度
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// 状态
        /// 0 待上传 1 上传中 2 已上传
        /// </summary>
        public int State { get; set; }
    }
}
