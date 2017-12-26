namespace SkyDrive.Client
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btn_upload = new DevExpress.XtraEditors.SimpleButton();
            this.panel_list = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panel_list)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_upload
            // 
            this.btn_upload.Dock = System.Windows.Forms.DockStyle.Top;
            this.btn_upload.Image = ((System.Drawing.Image)(resources.GetObject("btn_upload.Image")));
            this.btn_upload.ImageLocation = DevExpress.XtraEditors.ImageLocation.TopCenter;
            this.btn_upload.Location = new System.Drawing.Point(0, 0);
            this.btn_upload.Name = "btn_upload";
            this.btn_upload.Size = new System.Drawing.Size(737, 63);
            this.btn_upload.TabIndex = 0;
            this.btn_upload.Text = "上传";
            this.btn_upload.Click += new System.EventHandler(this.btn_upload_Click);
            // 
            // panel_list
            // 
            this.panel_list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_list.Location = new System.Drawing.Point(0, 63);
            this.panel_list.Name = "panel_list";
            this.panel_list.Size = new System.Drawing.Size(737, 384);
            this.panel_list.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 447);
            this.Controls.Add(this.panel_list);
            this.Controls.Add(this.btn_upload);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "网盘";
            ((System.ComponentModel.ISupportInitialize)(this.panel_list)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btn_upload;
        private DevExpress.XtraEditors.PanelControl panel_list;
    }
}

