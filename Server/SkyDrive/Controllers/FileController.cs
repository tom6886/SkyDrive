﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SkyDrive.Models;
using System;
using System.Threading.Tasks;

namespace SkyDrive.Controllers
{
    public class FileController : Controller
    {
        private IMongoCollection<Files> _files;

        private IMongoCollection<UserFiles> _userFiles;

        private MySettings _settings;

        public FileController(IMongoDatabase db, IOptions<MySettings> settings)
        {
            _files = db.GetCollection<Files>(nameof(Files));
            _userFiles = db.GetCollection<UserFiles>(nameof(UserFiles));
            _settings = settings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<string> MD5Check(string md5Str)
        {
            if (string.IsNullOrEmpty(md5Str)) { return JsonConvert.SerializeObject(new RequestResult() { Flag = -1, Message = "参数错误" }); }

            Files file = await _files.Find(Builders<Files>.Filter.Eq(q => q.MD5, md5Str)).FirstOrDefaultAsync();

            if (file == null) { return JsonConvert.SerializeObject(new RequestResult() { Flag = 0, Message = "不存在相同MD5" }); }

            return JsonConvert.SerializeObject(new RequestResult() { Flag = 1, Message = "存在相同MD5" });
        }

        [HttpPost]
        public async Task<string> NewUserFile(string userID, string md5Str, string fileName)
        {
            if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(md5Str) || string.IsNullOrEmpty(fileName)) { return JsonConvert.SerializeObject(new RequestResult() { Flag = -1, Message = "参数错误" }); }

            try
            {
                Files file = await _files.Find(Builders<Files>.Filter.Eq(q => q.MD5, md5Str)).FirstOrDefaultAsync();

                //如果不存在相同MD5的文件，则新建文件
                if (file == null)
                {
                    file = new Files()
                    {
                        Id = new ObjectId(),
                        MD5 = md5Str,
                        Position = 0,
                        State = 0
                    };

                    _files.InsertOne(file);
                }

                UserFiles userFile = new UserFiles()
                {
                    Id = new ObjectId(),
                    FileID = file.Id,
                    MD5 = md5Str,
                    FileName = fileName,
                    UserID = userID,
                    UploadTime = DateTime.Now
                };

                _userFiles.InsertOne(userFile);

                return JsonConvert.SerializeObject(new RequestResult() { Flag = 1, Message = "新建文件成功" });
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(new RequestResult() { Flag = -2, Message = "新建文件失败" });
            }
        }
    }
}