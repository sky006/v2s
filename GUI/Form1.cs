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
using com.dhu.cst.zjm;
using System.Text.RegularExpressions;

namespace v2s
{
    public partial class Form1 : Form
    {
        string input_file_name;
        public Form1()
        {
            InitializeComponent();
        }

        public Converter 使用
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        private void convert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(input_file_name))
            {
                message.Text = "未打开文件";
                return;
            }
            if (string.IsNullOrEmpty(src_lan.Text)|| string.IsNullOrEmpty(dst_lan.Text))
            {
                message.Text = "请选择语言";
                return;
            }
            Converter v_convert = new Converter();
            OutputPackage result = null;
            AudioFile vf = null;
            message.Text = "开始转换，请稍候...";
            if (IsVideo(input_file_name))
            {
                vf = v_convert.GetAudioFileFromVideo(input_file_name);
            }
            else
            {
                result = v_convert.ConvertToWAV(input_file_name);
            }
            message.Text = "转换完成，正在检查结果";
            string src = src_lan.Text;
            string dst = dst_lan.Text;
            string final_name;
            if (vf!=null)
            {
                final_name = vf.Path;
                message.Text = "转换成功。开始翻译...";
                if (Transformation(src, dst, Path.Combine(v_convert.WorkingPath, final_name)))
                {
                    message.Text = "翻译成功。";
                }
                else
                {
                    message.Text = "翻译失败。";
                }
                File.Delete(Path.Combine(v_convert.WorkingPath, final_name));
            }
            else if (result.Success)
            {
                using (MemoryStream mem = result.VideoStream)
                {
                    final_name = System.Guid.NewGuid().ToString() + ".wav";//转换结果
                    using (FileStream fs = new FileStream(Path.Combine(v_convert.WorkingPath, final_name), FileMode.OpenOrCreate))
                    {
                        byte[] buff = mem.ToArray();
                        fs.Write(buff, 0, buff.Length);
                        fs.Close();
                    }
                }
                message.Text = "转换成功。开始翻译...";
                if (Transformation(src, dst, Path.Combine(v_convert.WorkingPath, final_name)))
                {
                    message.Text = "翻译成功。";
                }
                else
                {
                    message.Text = "翻译失败。";
                }
                File.Delete(Path.Combine(v_convert.WorkingPath, final_name));
            }
            else
            {
                message.Text = "转换失败";
            }
        }
        //打开文件，获取文件名和长度
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog input_file = new OpenFileDialog();
            input_file.Filter = "音频文件|*.wav;*.mp3|视频文件|*.mp4;*.flv;*.avi;*.rmvb";
            if (input_file.ShowDialog() == DialogResult.OK)
            {
                input_file_name = input_file.FileName;

                Converter get_conver = new Converter();
                if (IsVideo(input_file_name))
                {
                    VideoFile vf = get_conver.GetVideoInfo(input_file_name);
                    video_len.Text = vf.Duration.ToString();
                }
                else
                {
                    AudioFile af = get_conver.GetAudioInfo(input_file_name);
                    video_len.Text = af.Duration.ToString();
                }
                src_lan.Text = "英语";
                dst_lan.Text = "汉语";
            }
        }
        /// <summary>
        /// 翻译
        /// </summary>
        /// <param name="src">源语言</param>
        /// <param name="dst">目标语言</param>
        /// <param name="path">源文件</param>
        public bool Transformation(string src,string dst,string path)
        {
            string dstFile = Path.Combine(Path.GetDirectoryName(input_file_name ), Path.GetFileNameWithoutExtension(input_file_name) + ".srt");
            string resFile = Path.Combine(Path.GetDirectoryName(path), "result.txt");
            if (dst=="汉语")
            {
                dst = "zh";
            }
            else
            {
                dst = "en";
            }
            if (src == "英语")
            {
                src = "en";
            }
            else
            {
                src = "zh";
            }
            string[] len = Regex.Split(video_len.Text, ":", RegexOptions.IgnoreCase);
            int length = Convert.ToInt32(len[0]) * 3600 + Convert.ToInt32(len[1]) * 60 + (int)(Convert.ToDouble(len[2]));
            iflyListener.transfer(path, length, resFile, dstFile,src,dst);
            if (File.Exists(dstFile))
            {
                var buffer = File.ReadAllBytes(dstFile);
                buffer = Encoding.Convert(Encoding.GetEncoding("GBK"), Encoding.UTF8, buffer);
                byte[] filebuffer = new byte[3 + (buffer.Length)];
                filebuffer[0] = Convert.ToByte("EF", 16);
                filebuffer[1] = Convert.ToByte("BB", 16);
                filebuffer[2] = Convert.ToByte("BF", 16);
                buffer.CopyTo(filebuffer, 3);
                File.WriteAllBytes(dstFile, filebuffer);
                File.Delete(resFile);
                return true;
            }
            File.Delete(resFile);
            return false;
        }
        /// <summary>
        /// 根据后缀判断文件类型
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool IsVideo(string fileName)
        {
            string[] vi = new string[] { ".mp4", ".flv", ".avi", ".rmvb" };
            if (Array.IndexOf(vi, Path.GetExtension(input_file_name)) != -1)
            {
                return true;
            }
            return false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
