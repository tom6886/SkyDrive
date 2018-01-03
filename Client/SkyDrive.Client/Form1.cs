using SkyDrive.Client.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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
                item.ChangeState += Item_ChangeState;
                item.Disposed += Item_Disposed;
                return item;
            });
        }

        private void Item_Disposed(object sender, EventArgs e)
        {
            FileListItem control = (FileListItem)sender;

            Task.Run(() => { RemoveFileInfo(control.ID); });

            lock (uploadList) { uploadList.Remove(control); }

            string temp = AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + control.FileName; //临时文件路径

            if (File.Exists(temp)) { File.Delete(temp); }
        }

        private void Item_ChangeState(FileListItem sender, int state)
        {


        }

        #region sqllite

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

        private void RemoveFileInfo(string id)
        {
            try
            {
                using (DBContext db = new DBContext())
                {
                    UploadFiles upload = db.UploadFiles.Where(q => q.ID.Equals(id)).FirstOrDefault();

                    if (upload == null) { LogHelper.WriteLog("保存文件信息时未查找到对应文件信息", null, null); return; }

                    db.UploadFiles.Remove(upload);

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
            try
            {
                using (DBContext db = new DBContext())
                {
                    UploadFiles upload = db.UploadFiles.Where(q => q.ID.Equals(id)).FirstOrDefault();

                    if (upload == null) { LogHelper.WriteLog("更新文件MD5时未查找到对应文件信息", null, null); return; }

                    upload.MD5 = MD5;

                    db.Entry(upload).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("更新文件MD5时出错", ex.Message, ex.StackTrace);
            }
        }

        private void SetFileServerID(string id, string serverFileID)
        {
            try
            {
                using (DBContext db = new DBContext())
                {
                    UploadFiles upload = db.UploadFiles.Where(q => q.ID.Equals(id)).FirstOrDefault();

                    if (upload == null) { LogHelper.WriteLog("更新文件服务端ID时未查找到对应文件信息", null, null); return; }

                    upload.ServerFileID = serverFileID;

                    db.Entry(upload).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("更新文件服务端ID时出错", ex.Message, ex.StackTrace);
            }
        }
        #endregion

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

            await Task.Run(() => SetFileMD5(id, MD5));

            RequestResult result = await Task.Run(() => { return RequestContext.Post("md5Check", "md5Str=" + MD5); });

            if (result == null) { SetFileItemMsg(control, "无法获取上传路径", Color.Red); return; }

            if (result.Flag < 0) { SetFileItemMsg(control, "验证MD5出错", Color.Red); return; }

            if (result.Flag == 1)
            {
                //若服务端存在相同MD5的文件，则开启极速秒传，仅上传用户信息
                SetFileItemMsg(control, "开启极速秒传..");

                //上传用户信息，结束后移除控件
                UploadUserFile(control, UploadInSecond);

                return;
            }

            //若服务端不存在相同MD5的文件，则复制本地文件到临时文件夹，准备上传
            await Task.Run(() => { CopyFile(control); });

            SetFileItemMsg(control, "开始上传...");

            //复制文件后上传用户文件信息
            UploadUserFile(control, UploadInNormal);
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

        #region 上传用户文件信息
        /// <summary>
        /// 上传文件信息之后事件的委托
        /// </summary>
        /// <param name="control"></param>
        private delegate void AfterUploadUserFile(FileListItem control, string fileID);

        private async void UploadUserFile(FileListItem control, AfterUploadUserFile afterUploadUserFile)
        {
            RequestResult uploadInfoResult = await Task.Run(() => { return RequestContext.Post("newUserFile", "userID=admin&md5Str=" + control.MD5 + "&fileName=" + control.FileName); });

            if (uploadInfoResult == null) { SetFileItemMsg(control, "无法获取上传文件信息路径", Color.Red); return; }

            if (uploadInfoResult.Flag < 0) { SetFileItemMsg(control, "初始化服务器文件出错", Color.Red); return; }

            afterUploadUserFile(control, uploadInfoResult.Data.ToString());
        }

        private void UploadInNormal(FileListItem control, string fileID)
        {
            Task.Run(() => { SetFileServerID(control.ID, fileID); });

            control.ServerFileID = fileID;

            control.UploadState = 1;
        }

        private void UploadInSecond(FileListItem control, string fileID)
        {
            control.Dispose();
        }
        #endregion

        #endregion

        #region 操作UI
        private delegate void setFileItemMsgHandler(FileListItem control, string msg, Color color);

        private void SetFileItemMsg(FileListItem control, string msg, Color color)
        {
            if (control.InvokeRequired)
            {
                BeginInvoke(new setFileItemMsgHandler(SetFileItemMsg), control, msg, color);
            }
            else
            {
                control.StateLabel.Text = msg;
                control.StateLabel.ForeColor = color;
            }
        }

        private void SetFileItemMsg(FileListItem control, string msg)
        {
            if (control.InvokeRequired)
            {
                BeginInvoke(new setFileItemMsgHandler(SetFileItemMsg), control, msg, Color.Black);
            }
            else
            {
                control.StateLabel.Text = msg;
                control.StateLabel.ForeColor = Color.Black;
            }
        }
        #endregion
    }
}
