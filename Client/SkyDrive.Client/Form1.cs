using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkyDrive.Client
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btn_upload_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"C:\";
            //ofd.Filter = "Image Files(*.JPG;*.PNG;*.jpeg;*.GIF;*.BMP)|*.JPG;*.PNG;*.GIF;*.BMP;*.jpeg|All files(*.*)|*.*";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(ofd.FileName)) { MessageBox.Show("未找到指定文件！"); return; }

                FileInfo file = new FileInfo(ofd.FileName);

                try
                {
                    FileListItem item = await NewFileListItemAsync(file);

                    panel_list.Controls.Add(item);

                    //首先校验文件MD5，若服务器存在相同文件，则实现极速秒传，若不存在，则将文件复制到临时文件夹，准备后续操作
                    SetFileItemMsg(item, "校验文件中...");

                    string md5Str = string.Empty;

                    double length = Math.Ceiling(file.Length / 1024.0);

                    //if (length < 102400 * 10)
                    //{
                    //    md5Str = MD5Checker.Check(ofd.FileName);
                    //}
                    //else
                    //{
                    //    MD5Checker checker = new MD5Checker();

                    //    checker.AsyncCheckProgress += Checker_AsyncCheckProgress;

                    //    checker.AsyncCheck(ofd.FileName);
                    //}

                    //将文件复制到临时文件夹
                    string temp = AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + file.Name; //临时文件路径

                    File.Copy(file.FullName, temp);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Checker_AsyncCheckProgress(AsyncCheckEventArgs e)
        {
            //if (e.State == AsyncCheckState.Checking)
            //{
            //    double pro = Convert.ToDouble(e.Value);
            //    if (pro - progress < 1) { return; }
            //    progress = pro;
            //    SetEdit("验算进度：" + pro + "%\r\n");
            //}
            //else
            //{
            //    SetEdit("验算完成，MD5值：" + e.Value + "\r\n");
            //    SetEdit("验算耗费时间：" + (DateTime.Now - beginTime).TotalSeconds + "s");
            //}
        }

        #region 方法
        private async Task<FileListItem> NewFileListItemAsync(FileInfo info)
        {
            return await Task.Run(() =>
            {
                FileListItem item = new FileListItem();
                item.FileName = info.Name;
                item.FileSize = info.Length;
                return item;
            });
        }

        
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
