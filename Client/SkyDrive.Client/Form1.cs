using SkyDrive.Client.Models;
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
                    FileListItem item = await NewFileListItemAsync(file);

                    lock (uploadList) { uploadList.Add(item); }

                    panel_list.Controls.Add(item);

                    //保存上传文件的数据到本地sqllite 方便中途关闭客户端后 再打开可以继续上传
                    SaveFileInfo(item.ID, file);

                    //对新上传的文件进行处理，首先校验文件MD5
                    //若服务器存在相同文件，则实现极速秒传，若不存在，则将文件复制到临时文件夹，准备后续操作
                    SetFileItemMsg(item, "校验文件中...");

                    MD5Check(item.ID, file);

                    //将文件复制到临时文件夹
                    //string temp = AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + file.Name; //临时文件路径

                    //File.Copy(file.FullName, temp);
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
                item.FileSize = info.Length;
                item.Dock = DockStyle.Top;
                return item;
            });
        }

        private void SaveFileInfo(string id, FileInfo file)
        {
            Task.Run(() =>
            {
                using (DBContext db = new DBContext())
                {
                    UploadFiles upload = new UploadFiles()
                    {
                        ID = id,
                        FileName = file.Name,
                        FileSource = file.FullName,
                        FileSize = file.Length
                    };

                    db.UploadFiles.Add(upload);

                    db.SaveChanges();
                }
            });
        }

        #region MD5
        private void MD5Check(string id, FileInfo file)
        {
            IMD5Checker checker = md5CheckerList.Where(q => q.State == 0).FirstOrDefault();

            if (checker == null)
            {
                checker = new IMD5Checker(id);
                checker.AsyncCheckProgress += Checker_AsyncCheckProgress;
                lock (md5CheckerList) { md5CheckerList.Add(checker); }
            }
            else
            {
                checker.ID = id;
                checker.State = 1;
                checker.Progress = 0;
            }

            double length = Math.Ceiling(file.Length / 1024.0);

            if (length < 1024 * 500)
            {
                string md5Str = checker.MD5Checker.Check(file.FullName);
                ServerMD5Check(id, md5Str);
            }
            else
            {
                //文件大于500M则启动异步验算
                checker.MD5Checker.AsyncCheck(file.FullName);
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
        private void ServerMD5Check(string id, string MD5)
        {
            FileListItem control = uploadList.Where(q => q.ID.Equals(id)).FirstOrDefault();

            if (control == null) { return; }

            SetFileItemMsg(control, "校验文件完毕..");

            RequestResult result = RequestContext.Post("md5Check", "md5Str=" + MD5);


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
