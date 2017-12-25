using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SkyDrive.Client
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_upload_Click(object sender, EventArgs e)
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


                    //首先校验文件MD5，若服务器存在相同文件，则实现极速秒传，若不存在，则将文件复制到临时文件夹，准备后续操作
                    double length = Math.Ceiling(file.Length / 1024.0);

                    //if (length < 102400 * 10)
                    //{
                    //    beginTime = DateTime.Now;
                    //    string md5Str = MD5Checker.Check(ofd.FileName);
                    //    memoEdit1.Text = "验算完成，MD5值：" + md5Str + "\r\n";
                    //    memoEdit1.MaskBox.AppendText("验算耗费时间：" + (DateTime.Now - beginTime).TotalSeconds + "s");
                    //}
                    //else
                    //{
                    //    memoEdit1.Text = "文件大小超过1G，开启异步计算\r\n";
                    //    progress = 0;
                    //    beginTime = DateTime.Now;
                    //    MD5Checker checker = new MD5Checker();
                    //    checker.AsyncCheckProgress += Checker_AsyncCheckProgress;
                    //    checker.AsyncCheck(ofd.FileName);
                    //}

                    //先将文件复制到临时文件夹
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
        private FileListItem AddFileListItem(FileInfo info)
        {
            FileListItem item = new FileListItem();
            item.FileName = info.Name;
            item.FileSize = info.Length;

            return item;
        }


        #endregion
    }
}
