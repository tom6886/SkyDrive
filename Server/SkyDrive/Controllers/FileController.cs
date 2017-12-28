using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SkyDrive.Models;
using System.Threading.Tasks;

namespace SkyDrive.Controllers
{
    public class FileController : Controller
    {
        private IMongoCollection<Files> files;

        public FileController(IMongoDatabase db)
        {
            files = db.GetCollection<Files>(nameof(Files));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<int> MD5Check(string md5Str)
        {
            if (string.IsNullOrEmpty(md5Str)) { return -1; }

            Files file = await files.Find(Builders<Files>.Filter.Eq(q => q.MD5, md5Str)).FirstOrDefaultAsync();

            if (file == null) { return 0; }

            return 1;
        }
    }
}