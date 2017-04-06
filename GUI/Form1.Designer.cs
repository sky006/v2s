namespace v2s
{
    partial class Form1
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.src_lan = new System.Windows.Forms.ComboBox();
            this.src_lan_label = new System.Windows.Forms.Label();
            this.dst_lan_label = new System.Windows.Forms.Label();
            this.dst_lan = new System.Windows.Forms.ComboBox();
            this.video_len_label = new System.Windows.Forms.Label();
            this.video_len = new System.Windows.Forms.Label();
            this.convert = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.message = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(284, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开ToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 打开ToolStripMenuItem
            // 
            this.打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
            this.打开ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.打开ToolStripMenuItem.Text = "打开";
            this.打开ToolStripMenuItem.Click += new System.EventHandler(this.打开ToolStripMenuItem_Click);
            // 
            // src_lan
            // 
            this.src_lan.FormattingEnabled = true;
            this.src_lan.Items.AddRange(new object[] {
            "汉语",
            "英语"});
            this.src_lan.Location = new System.Drawing.Point(92, 41);
            this.src_lan.Name = "src_lan";
            this.src_lan.Size = new System.Drawing.Size(121, 20);
            this.src_lan.TabIndex = 2;
            // 
            // src_lan_label
            // 
            this.src_lan_label.AutoSize = true;
            this.src_lan_label.Location = new System.Drawing.Point(47, 48);
            this.src_lan_label.Name = "src_lan_label";
            this.src_lan_label.Size = new System.Drawing.Size(41, 12);
            this.src_lan_label.TabIndex = 3;
            this.src_lan_label.Text = "源语言";
            // 
            // dst_lan_label
            // 
            this.dst_lan_label.AutoSize = true;
            this.dst_lan_label.Location = new System.Drawing.Point(35, 77);
            this.dst_lan_label.Name = "dst_lan_label";
            this.dst_lan_label.Size = new System.Drawing.Size(53, 12);
            this.dst_lan_label.TabIndex = 4;
            this.dst_lan_label.Text = "目标语言";
            // 
            // dst_lan
            // 
            this.dst_lan.FormattingEnabled = true;
            this.dst_lan.Items.AddRange(new object[] {
            "汉语",
            "英语"});
            this.dst_lan.Location = new System.Drawing.Point(92, 77);
            this.dst_lan.Name = "dst_lan";
            this.dst_lan.Size = new System.Drawing.Size(121, 20);
            this.dst_lan.TabIndex = 5;
            // 
            // video_len_label
            // 
            this.video_len_label.AutoSize = true;
            this.video_len_label.Location = new System.Drawing.Point(49, 131);
            this.video_len_label.Name = "video_len_label";
            this.video_len_label.Size = new System.Drawing.Size(53, 12);
            this.video_len_label.TabIndex = 6;
            this.video_len_label.Text = "文件时长";
            // 
            // video_len
            // 
            this.video_len.AutoSize = true;
            this.video_len.Location = new System.Drawing.Point(97, 131);
            this.video_len.Name = "video_len";
            this.video_len.Size = new System.Drawing.Size(0, 12);
            this.video_len.TabIndex = 7;
            // 
            // convert
            // 
            this.convert.Location = new System.Drawing.Point(51, 162);
            this.convert.Name = "convert";
            this.convert.Size = new System.Drawing.Size(75, 23);
            this.convert.TabIndex = 8;
            this.convert.Text = "转换";
            this.convert.UseVisualStyleBackColor = true;
            this.convert.Click += new System.EventHandler(this.convert_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // message
            // 
            this.message.AutoSize = true;
            this.message.Location = new System.Drawing.Point(51, 207);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(0, 12);
            this.message.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.message);
            this.Controls.Add(this.convert);
            this.Controls.Add(this.video_len);
            this.Controls.Add(this.video_len_label);
            this.Controls.Add(this.dst_lan);
            this.Controls.Add(this.dst_lan_label);
            this.Controls.Add(this.src_lan_label);
            this.Controls.Add(this.src_lan);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "v2s";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开ToolStripMenuItem;
        private System.Windows.Forms.ComboBox src_lan;
        private System.Windows.Forms.Label src_lan_label;
        private System.Windows.Forms.Label dst_lan_label;
        private System.Windows.Forms.ComboBox dst_lan;
        private System.Windows.Forms.Label video_len_label;
        private System.Windows.Forms.Label video_len;
        private System.Windows.Forms.Button convert;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label message;
    }
}

