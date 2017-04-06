using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Text.RegularExpressions;

namespace v2s
{
    public class Converter
    {
        #region 属性
        private string _ffExe;//ffmpeg路径
        public string ffExe
        {
            get
            {
                return _ffExe;
            }
            set
            {
                _ffExe = value;
            }
        }

        private string _WorkingPath;//工作路径
        public string WorkingPath
        {
            get
            {
                return _WorkingPath;
            }
            set
            {
                _WorkingPath = value;
            }
        }
        #endregion

        #region 构造函数
        public Converter()
        {
            Initialize();
        }
        public Converter(string ffmpegExePath)
        {
            _ffExe = ffmpegExePath;
            Initialize();
        }
        #endregion

        #region 初始化
        private void Initialize()
        {
            //确定存在ffmpeg的路径
            if (string.IsNullOrEmpty(_ffExe))
            {
                object o = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "workingfile","ffmpeg.exe");
                if (o == null)
                {
                    throw new Exception("Could not find the location of the ffmpeg exe file.  The path for ffmpeg.exe " +
                    "can be passed in via a constructor of the ffmpeg class (this class) or by setting in the app.config or web.config file.  " +
                    "in the appsettings section, the correct property name is: ffmpeg:ExeLocation");
                }
                else
                {
                    if (string.IsNullOrEmpty(o.ToString()))
                    {
                        throw new Exception("No value was found in the app setting for ffmpeg:ExeLocation");
                    }
                    _ffExe = o.ToString();
                }
            }

            //ffmpeg文件是否存在
            string workingpath = GetWorkingFile();
            if (string.IsNullOrEmpty(workingpath))
            {
                //ffmpeg doesn't exist at the location stated.
                throw new Exception("Could not find a copy of ffmpeg.exe");
            }
            _ffExe = workingpath;

            //是否已指定工作路径
            if (string.IsNullOrEmpty(_WorkingPath))
            {
                //object o = ConfigurationManager.AppSettings["ffmpeg:WorkingPath"];
                object o = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName,"workingpath");

                if (o != null)
                {
                    _WorkingPath = o.ToString();
                    if (Directory.Exists(_WorkingPath) == false)
                    {
                        Directory.CreateDirectory(_WorkingPath);
                    }
                }
                else
                {
                    _WorkingPath = string.Empty;
                }
            }
        }

        private string GetWorkingFile()
        {
            //尝试用绝对路径寻找ffmpeg文件
            if (File.Exists(_ffExe))
            {
                return _ffExe;
            }

            //尝试在根目录下寻找ffmpeg
            if (File.Exists(Path.GetFileName(_ffExe)))
            {
                return Path.GetFileName(_ffExe);
            }

            //未找到文件
            return null;
        }
        #endregion

        #region 获取图片文件，同时无需加锁
        public static System.Drawing.Image LoadImageFromFile(string fileName)
        {
            System.Drawing.Image theImage = null;
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open,
            FileAccess.Read))
            {
                byte[] img;
                img = new byte[fileStream.Length];
                fileStream.Read(img, 0, img.Length);
                fileStream.Close();
                theImage = System.Drawing.Image.FromStream(new MemoryStream(img));
                img = null;
            }
            GC.Collect();
            return theImage;
        }

        public static MemoryStream LoadMemoryStreamFromFile(string fileName)
        {
            MemoryStream ms = null;
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open,
            FileAccess.Read))
            {
                byte[] fil;
                fil = new byte[fileStream.Length];
                fileStream.Read(fil, 0, fil.Length);
                fileStream.Close();
                ms = new MemoryStream(fil);
            }
            GC.Collect();
            return ms;//返回内存流
        }
        #endregion

        #region 运行程序
        private string RunProcess(string Parameters)
        {
            //创建程序信息
            ProcessStartInfo oInfo = new ProcessStartInfo(this._ffExe, Parameters);
            oInfo.UseShellExecute = false;
            oInfo.CreateNoWindow = true;
            oInfo.RedirectStandardOutput = true;
            oInfo.RedirectStandardError = true;

            //保存输出结果
            string output = null;
            StreamReader srOutput = null;

            try
            {
                //运行程序
                Process proc = Process.Start(oInfo);

                proc.WaitForExit();

                //输出结果
                srOutput = proc.StandardError;

                //保存为字符串
                output = srOutput.ReadToEnd();

                proc.Close();
            }
            catch (Exception)
            {
                output = string.Empty;
            }
            finally
            {
                //成功
                if (srOutput != null)
                {
                    srOutput.Close();
                    srOutput.Dispose();
                }
            }
            return output;
        }
        #endregion

        #region 获取视频信息
        public VideoFile GetVideoInfo(MemoryStream inputFile, string Filename)
        {
            //构造临时文件
            string tempfile = Path.Combine(this.WorkingPath, System.Guid.NewGuid().ToString() + Path.GetExtension(Filename));
            FileStream fs = File.Create(tempfile);
            inputFile.WriteTo(fs);
            fs.Flush();
            fs.Close();
            GC.Collect();
            //将输入文件保存为vf
            VideoFile vf = null;
            try
            {
                vf = new VideoFile(tempfile);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            GetVideoInfo(vf);
            //释放资源
            try
            {
                File.Delete(tempfile);
            }
            catch (Exception)
            {

            }
            return vf;
        }

        public VideoFile GetVideoInfo(string inputPath)
        {
            VideoFile vf = null;
            try
            {
                vf = new VideoFile(inputPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            GetVideoInfo(vf);
            return vf;
        }

        public void GetVideoInfo(VideoFile input)
        {
            //设置参数
            string Params = string.Format("-i {0}", input.Path);
            string output = RunProcess(Params);
            input.RawInfo = output;

            //获取时长
            Regex re = new Regex("[D|d]uration:.((\\d|:|\\.)*)");
            Match m = re.Match(input.RawInfo);

            if (m.Success)
            {
                string duration = m.Groups[1].Value;
                string[] timepieces = duration.Split(new char[] { ':', '.' });
                if (timepieces.Length == 4)
                {
                    input.Duration = new TimeSpan(0, Convert.ToInt16(timepieces[0]), Convert.ToInt16(timepieces[1]), Convert.ToInt16(timepieces[2]), Convert.ToInt16(timepieces[3]));
                }
            }

            //获取比特率
            re = new Regex("[B|b]itrate:.((\\d|:)*)");
            m = re.Match(input.RawInfo);
            double kb = 0.0;
            if (m.Success)
            {
                Double.TryParse(m.Groups[1].Value, out kb);
            }
            input.BitRate = kb;

            //获取音频格式
            re = new Regex("[A|a]udio:.*");
            m = re.Match(input.RawInfo);
            if (m.Success)
            {
                input.AudioFormat = m.Value;
            }

            //获取视频格式
            re = new Regex("[V|v]ideo:.*");
            m = re.Match(input.RawInfo);
            if (m.Success)
            {
                input.VideoFormat = m.Value;
            }

            //获取视频格式（分辨率）
            re = new Regex("(\\d{2,3})x(\\d{2,3})");
            m = re.Match(input.RawInfo);
            if (m.Success)
            {
                int width = 0; int height = 0;
                int.TryParse(m.Groups[1].Value, out width);
                int.TryParse(m.Groups[2].Value, out height);
                input.Width = width;
                input.Height = height;
            }
            input.infoGathered = true;
        }
        #endregion

        #region 获取音频信息
        public AudioFile GetAudioInfo(MemoryStream inputFile, string Filename)
        {
            //构造临时文件
            string tempfile = Path.Combine(this.WorkingPath, System.Guid.NewGuid().ToString() + Path.GetExtension(Filename));
            FileStream fs = File.Create(tempfile);
            inputFile.WriteTo(fs);
            fs.Flush();
            fs.Close();
            GC.Collect();
            //将输入文件保存为vf
            AudioFile af = null;
            try
            {
                af = new AudioFile(tempfile);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            GetAudioInfo(af);
            //释放资源
            try
            {
                File.Delete(tempfile);
            }
            catch (Exception)
            {

            }
            return af;
        }

        public AudioFile GetAudioInfo(string inputPath)
        {
            AudioFile af = null;
            try
            {
                af = new AudioFile(inputPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            GetAudioInfo(af);
            return af;
        }

        public void GetAudioInfo(AudioFile input)
        {
            //设置参数
            string Params = string.Format("-i {0}", input.Path);
            string output = RunProcess(Params);
            input.RawInfo = output;

            //获取时长
            Regex re = new Regex("[D|d]uration:.((\\d|:|\\.)*)");
            Match m = re.Match(input.RawInfo);

            if (m.Success)
            {
                string duration = m.Groups[1].Value;
                string[] timepieces = duration.Split(new char[] { ':', '.' });
                if (timepieces.Length == 4)
                {
                    input.Duration = new TimeSpan(0, Convert.ToInt16(timepieces[0]), Convert.ToInt16(timepieces[1]), Convert.ToInt16(timepieces[2]), Convert.ToInt16(timepieces[3]));
                }
            }

            //获取比特率
            re = new Regex("[B|b]itrate:.((\\d|:)*)");
            m = re.Match(input.RawInfo);
            double kb = 0.0;
            if (m.Success)
            {
                Double.TryParse(m.Groups[1].Value, out kb);
            }
            input.BitRate = kb;

            //获取音频格式
            re = new Regex("[A|a]udio:.*");
            m = re.Match(input.RawInfo);
            if (m.Success)
            {
                input.AudioFormat = m.Value;
            }
           
            input.infoGathered = true;
        }
        #endregion

        #region 转为FLV文件
        public OutputPackage ConvertToFLV(MemoryStream inputFile, string Filename)
        {
            //建临时文件
            string tempfile = Path.Combine(this.WorkingPath, System.Guid.NewGuid().ToString() + Path.GetExtension(Filename));
            FileStream fs = File.Create(tempfile);
            inputFile.WriteTo(fs);
            fs.Flush();
            fs.Close();
            GC.Collect();
            //文件保存为vf
            VideoFile vf = null;
            try
            {
                vf = new VideoFile(tempfile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //结果为outputpackage格式
            OutputPackage oo = ConvertToFLV(vf);

            try
            {
                File.Delete(tempfile);
            }
            catch (Exception)
            {

            }

            return oo;
        }

        public OutputPackage ConvertToFLV(string inputPath)
        {
            VideoFile vf = null;
            try
            {
                vf = new VideoFile(inputPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            OutputPackage oo = ConvertToFLV(vf);
            return oo;
        }

        public OutputPackage ConvertToFLV(VideoFile input)
        {
            if (!input.infoGathered)
            {
                GetVideoInfo(input);
            }
            OutputPackage ou = new OutputPackage();

            //设置参数获取预览图像
            string filename = System.Guid.NewGuid().ToString() + ".jpg";
            int secs;

            //计算视频1/3有多少秒，获取该处的图像作为预览图
            secs = (int)Math.Round(TimeSpan.FromTicks(input.Duration.Ticks / 3).TotalSeconds, 0);

            string finalpath = Path.Combine(this.WorkingPath, filename);
            string Params = string.Format("-i {0} {1} -vcodec mjpeg -ss {2} -vframes 1 -an -f rawvideo", input.Path, finalpath, secs);
            string output = RunProcess(Params);

            ou.RawOutput = output;
            //临时文件存在，说明创建成功
            if (File.Exists(finalpath))
            {
                ou.PreviewImage = LoadImageFromFile(finalpath);
                try
                {
                    File.Delete(finalpath);
                }
                catch (Exception) { }
            }
            else
            { //尝试获取1秒处图像作为预览图
                Params = string.Format("-i {0} {1} -vcodec mjpeg -ss {2} -vframes 1 -an -f rawvideo", input.Path, finalpath, 1);
                output = RunProcess(Params);

                ou.RawOutput = output;

                if (File.Exists(finalpath))
                {
                    ou.PreviewImage = LoadImageFromFile(finalpath);
                    try
                    {
                        File.Delete(finalpath);
                    }
                    catch (Exception) { }
                }
            }

            filename = System.Guid.NewGuid().ToString() + ".flv";
            finalpath = Path.Combine(this.WorkingPath, filename);
            Params = string.Format("-i {0} -y -ar 22050 -ab 64 -f flv {1}", input.Path, finalpath);
            output = RunProcess(Params);
            //是否转换成功
            if (File.Exists(finalpath))
            {
                ou.VideoStream = LoadMemoryStreamFromFile(finalpath);
                try
                {
                    File.Delete(finalpath);
                }
                catch (Exception) { }
            }
            return ou;
        }
        #endregion

        #region 从视频中提取音频
        public AudioFile GetAudioFileFromVideo(string inputPath)
        {
            VideoFile vf = null;
            try
            {
                vf = new VideoFile(inputPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return GetAudioFileFromVideo(vf);
        }
        public AudioFile GetAudioFileFromVideo(VideoFile vf)
        {         
            if (!vf.infoGathered)
            {
                GetVideoInfo(vf);
            }
            //首先提取音频为MP3
            string filename = System.Guid.NewGuid().ToString() + ".wav";
            string finalpath = Path.Combine(this.WorkingPath, filename);
            string Params = string.Format("-i {0} -f wav -ar 22050 -ac 1 {1}", vf.Path, finalpath);
            
            string output = RunProcess(Params);
            //是否提取成功
            if (File.Exists(finalpath))
            {
                return new AudioFile(finalpath);
            }
            return null;
        }
        #endregion

        #region 转为WAV文件
        public OutputPackage ConvertToWAV(MemoryStream inputFile, string Filename)
        {
            //建临时文件
            string tempfile = Path.Combine(this.WorkingPath, System.Guid.NewGuid().ToString() + Path.GetExtension(Filename));
            FileStream fs = File.Create(tempfile);
            inputFile.WriteTo(fs);
            fs.Flush();
            fs.Close();
            GC.Collect();

            //文件保存为vf
            AudioFile af = null;
            try
            {
                af = new AudioFile(tempfile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //结果为outputpackage格式
            OutputPackage oo = ConvertToWAV(af);

            try
            {
                File.Delete(tempfile);
            }
            catch (Exception)
            {

            }
            return oo;
        }

        public OutputPackage ConvertToWAV(string inputPath)
        {
            AudioFile af = null;
            try
            {
                af = new AudioFile(inputPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            OutputPackage oo = ConvertToWAV(af);
            return oo;
        }

        public OutputPackage ConvertToWAV(AudioFile input)
        {
            if (!input.infoGathered)
            {
                GetAudioInfo(input);
            }
            OutputPackage ou = new OutputPackage();
            //最终结果保存的文件路径
            string filename = System.Guid.NewGuid().ToString() + ".wav";
            string finalpath = Path.Combine(this.WorkingPath, filename);
            //转化
            string Params = string.Format("-i {0} -ar 22050 -ac 1 -acodec pcm_s16le {1}", input.Path, finalpath);
            string output = RunProcess(Params);
            //是否转换成功
            if (File.Exists(finalpath))
            {
                ou.VideoStream = LoadMemoryStreamFromFile(finalpath);
                ou.Success = true;
                try
                {
                    File.Delete(finalpath);
                }
                catch (Exception) { }
            }
            return ou;
        }
        #endregion

        public AudioFile 音频文件
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public VideoFile 视频文件
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public OutputPackage 输出结果
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }
    }


    public class VideoFile
    {
        #region 属性
        private string _Path;


        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
            }
        }

        public TimeSpan Duration { get; set; }
        public double BitRate { get; set; }
        public string AudioFormat { get; set; }
        public string VideoFormat { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string RawInfo { get; set; }
        public bool infoGathered { get; set; }
        #endregion

        #region 构造函数
        public VideoFile(string path)
        {
            _Path = path;
            Initialize();
        }
        #endregion

        #region 初始化
        private void Initialize()
        {
            this.infoGathered = false;
            //判断是否提供了路径
            if (string.IsNullOrEmpty(_Path))
            {
                throw new Exception("Could not find the location of the video file");
            }

            //文件是否存在
            if (!File.Exists(_Path))
            {
                throw new Exception("The video file " + _Path + " does not exist.");
            }
        }
        #endregion
    }
    public class AudioFile
    {
        #region 属性
        private string _Path;
        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
            }
        }

        public TimeSpan Duration { get; set; }
        public double BitRate { get; set; }
        public string AudioFormat { get; set; }
        public string RawInfo { get; set; }
        public bool infoGathered { get; set; }
        #endregion

        #region 构造函数
        public AudioFile(string path)
        {
            _Path = path;
            Initialize();
        }
        #endregion

        #region 初始化
        private void Initialize()
        {
            this.infoGathered = false;
            //判断是否提供了路径
            if (string.IsNullOrEmpty(_Path))
            {
                throw new Exception("Could not find the location of the audio file");
            }

            //文件是否存在
            if (!File.Exists(_Path))
            {
                throw new Exception("The audio file " + _Path + " does not exist.");
            }
        }
        #endregion
    }
    /// <summary>
    /// 用于输出结果的类
    /// </summary>
    public class OutputPackage
    {
        public MemoryStream VideoStream { get; set; }
        public System.Drawing.Image PreviewImage { get; set; }
        public string RawOutput { get; set; }
        public bool Success { get; set; }
        #region 构造函数
        public OutputPackage()
        {
            Success = false;
        }
        #endregion
    }
}
