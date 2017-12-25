using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using SkyDrive.Client.Properties;
using System;

namespace SkyDrive.Client
{
    public partial class FileListItem : UserControl
    {
        #region 属性
        [Browsable(true)]
        [Description("文件名"), Category("FileName"), DefaultValue("")]
        public string FileName
        {
            set
            {
                lb_name.Text = value;
            }
            get
            {
                return lb_name.Text;
            }
        }

        private long _FileSize = 0;

        [Browsable(true)]
        [Description("文件大小"), Category("FileSize"), DefaultValue("")]
        public long FileSize
        {
            set
            {
                _FileSize = value;

                lb_size.Text = CountSize(value);
            }
            get
            {
                return _FileSize;
            }
        }

        private int _UploadState = 0;

        [Browsable(true)]
        [Description("上传状态"), Category("UploadState"), DefaultValue("")]
        public int UploadState
        {
            set
            {
                btn_play.Image = value == 0 ? Resources.pause : Resources.start;
            }
            get
            {
                return _UploadState;
            }
        }

        [Browsable(true)]
        [Description("上传进度"), Category("UploadProgress"), DefaultValue("")]
        public int UploadProgress
        {
            set
            {
                progress.Position = value;
            }
            get
            {
                return progress.Position;
            }
        }

        public LabelControl StateLabel { get { return lb_msg; } }
        #endregion

        public FileListItem()
        {
            InitializeComponent();
        }

        #region 方法

        private string CountSize(double size)
        {
            string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
            double mod = 1024.0;
            int i = 0;
            while (size >= mod)
            {
                size /= mod;
                i++;
            }
            return Math.Round(size) + units[i];
        }
        #endregion
    }
}
