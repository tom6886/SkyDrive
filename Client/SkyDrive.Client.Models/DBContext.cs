using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyDrive.Client.Models
{
    public class DBContext : DbContext
    {
        public DBContext()
            : base("SqliteDB")
        {

        }

        public DbSet<UploadFiles> UploadFiles { get; set; }
    }
}
