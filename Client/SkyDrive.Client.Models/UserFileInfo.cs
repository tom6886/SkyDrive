namespace SkyDrive.Models
{
    public class UserFileInfo
    {
        public string FileName { get; set; }

        public string MD5 { get; set; }

        public long Position { get; set; }

        public int State { get; set; }
    }
}
