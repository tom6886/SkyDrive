using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkyDrive.Client.Models
{
    [Table("UploadFiles")]
    public class UploadFiles
    {
        [Key]
        public string ID { get; set; }

        public string FileName { get; set; }

        public string FileSource { get; set; }

        public long FileSize { get; set; }

        /// <summary>
        /// 文件当前状态：
        /// 0-初始化
        /// 1-验算MD5中
        /// 2-上传中
        /// 3-已上传
        /// </summary>
        public int State { get; set; }
    }
}
