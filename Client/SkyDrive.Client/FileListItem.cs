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
                if (value == _UploadState) { return; }

                _UploadState = value;
                btn_play.Image = value > 0 ? Resources.pause : Resources.start;
                ChangeState?.Invoke(this, value);
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

        public string ID { get; set; }

        public string ServerFileID { get; set; }

        public string MD5 { get; set; }

        private int _State = 0;

        public int State { get { return _State; } set { _State = value; btn_play.Enabled = value > 0; } }

        public LabelControl StateLabel { get { return lb_msg; } }

        public string FileSource { get; set; }

        public string BackUpName { get; set; }

        public System.Timers.Timer UploadTimer { get; set; }
        #endregion

        #region 委托和事件
        /// <summary>
        /// 上传状态改变的委托
        /// </summary>
        /// <param name="sender"></param>
        public delegate void ChangeStateHandler(FileListItem sender, int state);
        /// <summary>
        /// 上传状态改变的事件
        /// </summary>
        public event ChangeStateHandler ChangeState;
        /// <summary>
        /// 上传定时委托
        /// </summary>
        /// <param name="sender"></param>
        public delegate void UploadTimerElapsedHandler(FileListItem sender, System.Timers.ElapsedEventArgs e);
        /// <summary>
        /// 上传定时事件
        /// </summary>
        public event UploadTimerElapsedHandler UploadTimerElapsed;
        #endregion

        #region 私有方法

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

        public FileListItem()
        {
            InitializeComponent();
        }

        private void btn_play_Click(object sender, EventArgs e)
        {
            UploadState = _UploadState == 0 ? 1 : 0;
        }

        public System.Timers.Timer CreateUploadTimer()
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;

            this.UploadTimer = timer;

            return timer;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UploadTimerElapsed?.Invoke(this, e);
        }
    }
}
