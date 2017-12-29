using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
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
        public async Task<string> MD5Check(string md5Str)
        {
            if (string.IsNullOrEmpty(md5Str)) { return JsonConvert.SerializeObject(new RequestResult() { Flag = -1, Message = "参数错误" }); }

            Files file = await files.Find(Builders<Files>.Filter.Eq(q => q.MD5, md5Str)).FirstOrDefaultAsync();

            if (file == null) { return JsonConvert.SerializeObject(new RequestResult() { Flag = 0, Message = "不存在相同MD5" }); }

            return JsonConvert.SerializeObject(new RequestResult() { Flag = 1, Message = "存在相同MD5" });
        }
    }
}