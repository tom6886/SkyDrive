namespace SkyDrive.Models
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; }

        public string DataBaseName { get; set; }

        public string FilesCollectionName { get; set; }

        public string UserFilesCollectionName { get; set; }
    }
}
