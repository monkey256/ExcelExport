using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace tablegen2
{
    internal static class Util
    {
        /// <summary>
        /// 获取环境目录
        /// </summary>
        public static string WorkPath
        {
            get { return System.AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static string ModulePath
        {
            get { return System.Windows.Forms.Application.ExecutablePath; }
        }

        /// <summary>
        /// 确保此文件夹存在
        /// </summary>
        /// <param name="dir">文件夹路径</param>
        public static void MakesureFolderExist(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="dir">目录</param>
        public static void OpenDir(string dir)
        {
            try
            {
                if (!System.IO.Directory.Exists(dir))
                    return;

                System.Diagnostics.Process.Start("explorer.exe", dir);
            }
            catch (System.Exception)
            {
            }
        }

        /// <summary>
        /// 打开某个目录并选中对应文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public static void OpenDirFile(string path)
        {
            try
            {
                if (!System.IO.File.Exists(path) && !System.IO.Directory.Exists(path))
                    return;

                System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", path));
            }
            catch (System.Exception)
            {
            }
        }

        /// <summary>
        /// 获取1970到现在的天数
        /// </summary>
        /// <returns></returns>
        public static int GetDayFrom1970()
        {
            DateTime tm = new DateTime(1970, 1, 1);
            TimeSpan ts = DateTime.Now - tm;
            return ts.Days;
        }

        /// <summary>
        /// 取得随机数
        /// </summary>
        private static Random rdm_ = new Random(Environment.TickCount);
        public static int GetRandom(int min, int max)
        {
            return rdm_.Next(min, max);
        }

        /// <summary>
        /// 计算缓冲区的Md5
        /// </summary>
        public static string MdBuffer(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder();
            MD5CryptoServiceProvider svr = new MD5CryptoServiceProvider();
            foreach (byte b in svr.ComputeHash(buffer))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        /// <summary>
        /// 计算字符串的UTF8格式Md5值
        /// </summary>
        public static string MdString(string str)
        {
            return MdBuffer(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// 计算文件的Md5值
        /// </summary>
        public static string MdFile(string path)
        {
            using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                StringBuilder sb = new StringBuilder();
                MD5CryptoServiceProvider svr = new MD5CryptoServiceProvider();
                foreach (byte b in svr.ComputeHash(fs))
                    sb.Append(b.ToString("X2"));
                return sb.ToString();
            }
        }

        public static string ToFormatLength(long length)
        {
            string s = "";
            if (length < 1024)
            {
                s = String.Format("{0} KB", Math.Ceiling(length / 1024.0f));
            }
            else if (length < 1024 * 1024)
            {
                s = String.Format("{0:N2} KB", length / 1024.0f);
            }
            else if (length < 1024 * 1024 * 1024)
            {
                s = String.Format("{0:N2} MB", length / 1024.0f / 1024.0f);
            }
            else
            {
                s = String.Format("{0:N2} MB", length / 1024.0f / 1024.0f / 1024.0f);
            }

            return s;
        }

        /// <summary>
        /// 删除某个目录，最大删除清理
        /// </summary>
        /// <param name="dir">要删除的目录</param>
        /// <returns>成功或失败</returns>
        public static bool RemoveDirectory(string dir)
        {
            bool r = true;

            if (Directory.Exists(dir))
            {
                foreach (var s in Directory.GetFiles(dir))
                {
                    try
                    {
                        File.Delete(s);
                        Thread.Sleep(1);
                    }
                    catch
                    {
                        r = false;
                    }
                }

                foreach (var s in Directory.GetDirectories(dir))
                {
                    if (!RemoveDirectory(s))
                    {
                        r = false;
                        continue;
                    }

                    Directory.Delete(s);
                    Thread.Sleep(1);
                }
            }

            return r;
        }

        public static bool IsStringNumber(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            foreach (var c in s)
            {
                if (!char.IsNumber(c))
                    return false;
            }
            return true;
        }

        public static void performClick(System.Windows.Controls.Button btn)
        {
            btn.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, btn));
        }
    }
}
