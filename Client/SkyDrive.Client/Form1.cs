﻿using SkyDrive.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using XUtils;

namespace SkyDrive.Client
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 上传控件列表
        /// </summary>
        private List<FileListItem> uploadList = new List<FileListItem>();

        /// <summary>
        /// MD5校验对象列表
        /// </summary>
        private List<IMD5Checker> md5CheckerList = new List<IMD5Checker>();

        public Form1()
        {
            InitializeComponent();
        }

        private async void btn_upload_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"D:\迅雷下载";
            //ofd.Filter = "Image Files(*.JPG;*.PNG;*.jpeg;*.GIF;*.BMP)|*.JPG;*.PNG;*.GIF;*.BMP;*.jpeg|All files(*.*)|*.*";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(ofd.FileName)) { MessageBox.Show("未找到指定文件！"); return; }

                FileInfo file = new FileInfo(ofd.FileName);

                try
                {
                    //todo 若所选文件已存在于上传列表时 询问是否重新上传

                    FileListItem item = await NewFileListItemAsync(file);

                    lock (uploadList) { uploadList.Add(item); }

                    panel_list.Controls.Add(item);

                    //保存上传文件的数据到本地sqllite 方便中途关闭客户端后 再打开可以继续上传
                    await Task.Run(() => SaveFileInfo(item));

                    //对新上传的文件进行处理，首先校验文件MD5
                    //若服务器存在相同文件，则实现极速秒传，若不存在，则将文件复制到临时文件夹，准备后续操作
                    SetFileItemMsg(item, "校验文件中...");

                    MD5Check(item);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        #region 方法
        private async Task<FileListItem> NewFileListItemAsync(FileInfo info)
        {
            return await Task.Run(() =>
            {
                FileListItem item = new FileListItem();
                item.ID = StringUtil.UniqueID();
                item.FileName = info.Name;
                item.BackUpName = info.Name;
                item.FileSize = info.Length;
                item.FileSource = info.FullName;
                item.Dock = DockStyle.Top;
                return item;
            });
        }

        private void SaveFileInfo(FileListItem item)
        {
            try
            {
                using (DBContext db = new DBContext())
                {
                    UploadFiles upload = new UploadFiles()
                    {
                        ID = item.ID,
                        FileName = item.FileName,
                        BackUpName = item.BackUpName,
                        FileSource = item.FileSource,
                        FileSize = item.FileSize
                    };

                    db.UploadFiles.Add(upload);

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("保存文件信息时出错", ex.Message, ex.StackTrace);
            }
        }

        private void SetFileMD5(string id, string MD5)
        {
            Task.Run(() =>
            {
                try
                {
                    using (DBContext db = new DBContext())
                    {
                        UploadFiles upload = db.UploadFiles.Where(q => q.ID.Equals(id)).FirstOrDefault();

                        if (upload == null) { return; }

                        upload.MD5 = MD5;

                        db.Entry(upload).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("更新文明MD5时出错", ex.Message, ex.StackTrace);
                }
            });
        }

        #region MD5
        private void MD5Check(FileListItem item)
        {
            IMD5Checker checker = md5CheckerList.Where(q => q.State == 0).FirstOrDefault();

            if (checker == null)
            {
                checker = new IMD5Checker(item.ID);
                checker.AsyncCheckProgress += Checker_AsyncCheckProgress;
                lock (md5CheckerList) { md5CheckerList.Add(checker); }
            }
            else
            {
                checker.ID = item.ID;
                checker.State = 1;
                checker.Progress = 0;
            }

            double length = Math.Ceiling(item.FileSize / 1024.0);

            if (length < 1024 * 500)
            {
                string md5Str = checker.MD5Checker.Check(item.FileSource);
                ServerMD5Check(item.ID, md5Str);
            }
            else
            {
                //文件大于500M则启动异步验算
                checker.MD5Checker.AsyncCheck(item.FileSource);
            }
        }

        private void Checker_AsyncCheckProgress(IMD5Checker sender, AsyncCheckEventArgs e)
        {
            if (e.State == AsyncCheckState.Checking)
            {
                double pro = Convert.ToDouble(e.Value);
                if (pro - sender.Progress < 1) { return; }
                sender.Progress = pro;
                FileListItem control = uploadList.Where(q => q.ID.Equals(sender.ID)).FirstOrDefault();
                if (control == null) { return; }
                SetFileItemMsg(control, "校验文件中,进度：" + pro + "%");
            }
            else
            {
                ServerMD5Check(sender.ID, e.Value);
            }
        }

        /// <summary>
        /// 上传MD5到服务端 查看是否已存在相同文件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="MD5"></param>
        private async void ServerMD5Check(string id, string MD5)
        {
            FileListItem control = uploadList.Where(q => q.ID.Equals(id)).FirstOrDefault();

            if (control == null) { return; }

            SetFileItemMsg(control, "校验文件完毕，准备上传..");

            //更新对应控件以及sqllite中对应文件信息的MD5值
            control.MD5 = MD5;

            SetFileMD5(id, MD5);

            RequestResult result = RequestContext.Post("md5Check", "md5Str=" + MD5);

            if (result == null) { MessageBox.Show("无法获取上传路径，请联系管理员"); return; }

            if (result.Flag == 1)
            {
                //若服务端存在相同MD5的文件，则开启极速秒传，仅上传用户信息
                SetFileItemMsg(control, "开启极速秒传..");

                //todo 上传用户信息，结束后移除控件
            }

            //若服务端不存在相同MD5的文件，则复制本地文件到临时文件夹，准备上传
            await Task.Run(() => { CopyFile(control); });


        }
        #endregion

        #region 操作文件
        private void CopyFile(FileListItem control)
        {
            string temp = AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + control.FileName; //临时文件路径

            //若临时文件夹中已存在同名文件，则文件名加当前时间点保存为备用文件名
            if (File.Exists(temp))
            {
                int i = control.FileName.LastIndexOf(".");

                string backUpName = control.FileName.Substring(0, i) + DateTime.Now.ToString("yyyyMMddHHmmss") + control.FileName.Substring(i);

                temp = AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + backUpName;

                control.BackUpName = backUpName;

                using (DBContext db = new DBContext())
                {
                    UploadFiles upload = db.UploadFiles.Where(q => q.ID.Equals(control.ID)).FirstOrDefault();

                    if (upload == null) { SetFileItemMsg(control, "未找到对应上传记录"); }

                    upload.BackUpName = backUpName;

                    db.Entry(upload).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
            }

            File.Copy(control.FileSource, temp);
        }
        #endregion

        #endregion

        #region 操作UI
        private delegate void setFileItemMsgHandler(FileListItem control, string msg);

        private void SetFileItemMsg(FileListItem control, string msg)
        {
            if (control.InvokeRequired)
            {
                BeginInvoke(new setFileItemMsgHandler(SetFileItemMsg), control, msg);
            }
            else
            {
                control.StateLabel.Text = msg;
            }
        }


        #endregion
    }
}
