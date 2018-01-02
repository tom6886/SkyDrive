namespace SkyDrive.Client
{
    partial class FileListItem
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btn_open = new System.Windows.Forms.PictureBox();
            this.btn_delete = new System.Windows.Forms.PictureBox();
            this.btn_play = new System.Windows.Forms.PictureBox();
            this.lb_msg = new DevExpress.XtraEditors.LabelControl();
            this.progress = new DevExpress.XtraEditors.ProgressBarControl();
            this.lb_size = new DevExpress.XtraEditors.LabelControl();
            this.lb_name = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_open)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_delete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_play)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.progress.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.panelControl1.Controls.Add(this.btn_open);
            this.panelControl1.Controls.Add(this.btn_delete);
            this.panelControl1.Controls.Add(this.btn_play);
            this.panelControl1.Controls.Add(this.lb_msg);
            this.panelControl1.Controls.Add(this.progress);
            this.panelControl1.Controls.Add(this.lb_size);
            this.panelControl1.Controls.Add(this.lb_name);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(700, 55);
            this.panelControl1.TabIndex = 2;
            // 
            // btn_open
            // 
            this.btn_open.Image = global::SkyDrive.Client.Properties.Resources.folder;
            this.btn_open.Location = new System.Drawing.Point(630, 15);
            this.btn_open.Name = "btn_open";
            this.btn_open.Size = new System.Drawing.Size(24, 24);
            this.btn_open.TabIndex = 6;
            this.btn_open.TabStop = false;
            // 
            // btn_delete
            // 
            this.btn_delete.Image = global::SkyDrive.Client.Properties.Resources.delete;
            this.btn_delete.Location = new System.Drawing.Point(570, 15);
            this.btn_delete.Name = "btn_delete";
            this.btn_delete.Size = new System.Drawing.Size(24, 24);
            this.btn_delete.TabIndex = 5;
            this.btn_delete.TabStop = false;
            // 
            // btn_play
            // 
            this.btn_play.Enabled = false;
            this.btn_play.Image = global::SkyDrive.Client.Properties.Resources.start;
            this.btn_play.Location = new System.Drawing.Point(510, 15);
            this.btn_play.Name = "btn_play";
            this.btn_play.Size = new System.Drawing.Size(24, 24);
            this.btn_play.TabIndex = 4;
            this.btn_play.TabStop = false;
            this.btn_play.Click += new System.EventHandler(this.btn_play_Click);
            // 
            // lb_msg
            // 
            this.lb_msg.Location = new System.Drawing.Point(244, 29);
            this.lb_msg.Name = "lb_msg";
            this.lb_msg.Size = new System.Drawing.Size(0, 14);
            this.lb_msg.TabIndex = 3;
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(244, 5);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(214, 18);
            this.progress.TabIndex = 2;
            // 
            // lb_size
            // 
            this.lb_size.Location = new System.Drawing.Point(5, 29);
            this.lb_size.Name = "lb_size";
            this.lb_size.Size = new System.Drawing.Size(48, 14);
            this.lb_size.TabIndex = 1;
            this.lb_size.Text = "文件大小";
            // 
            // lb_name
            // 
            this.lb_name.Appearance.Font = new System.Drawing.Font("Tahoma", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_name.Location = new System.Drawing.Point(5, 6);
            this.lb_name.Name = "lb_name";
            this.lb_name.Size = new System.Drawing.Size(42, 17);
            this.lb_name.TabIndex = 0;
            this.lb_name.Text = "文件名";
            // 
            // FileListItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Name = "FileListItem";
            this.Size = new System.Drawing.Size(700, 55);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_open)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_delete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_play)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.progress.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl lb_size;
        private DevExpress.XtraEditors.LabelControl lb_name;
        private DevExpress.XtraEditors.ProgressBarControl progress;
        private DevExpress.XtraEditors.LabelControl lb_msg;
        private System.Windows.Forms.PictureBox btn_play;
        private System.Windows.Forms.PictureBox btn_open;
        private System.Windows.Forms.PictureBox btn_delete;
    }
}
