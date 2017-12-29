using System;
using System.IO;
using System.Security.Cryptography;

namespace SkyDrive.Client
{
    internal class MD5Checker
    {
        #region 属性
        //支持所有哈希算法  
        private HashAlgorithm hashAlgorithm;
        //文件缓冲区  
        private byte[] buffer;
        //文件读取流  
        private Stream inputStream;
        #endregion

        #region 委托与方法
        /// <summary>
        /// 异步读取文件的委托
        /// </summary>
        /// <param name="e"></param>
        internal delegate void AsyncCheckHandler(AsyncCheckEventArgs e);
        /// <summary>
        /// 异步读取文件的方法
        /// </summary>
        internal event AsyncCheckHandler AsyncCheckProgress;
        #endregion

        internal string Check(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException(string.Format("<{0}>, 不存在", path));

            FileStream file = new FileStream(path, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
            return BitConverter.ToString(retVal).Replace("-", "");
        }

        internal void AsyncCheck(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException(string.Format("<{0}>, 不存在", path));

            int bufferSize = 1048576;//缓冲区大小，1MB  

            buffer = new byte[bufferSize];

            //打开文件流  
            inputStream = File.Open(path, FileMode.Open);
            hashAlgorithm = new MD5CryptoServiceProvider();

            //异步读取数据到缓冲区  
            inputStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(AsyncComputeHashCallback), null);
        }

        internal void AsyncComputeHashCallback(IAsyncResult result)
        {
            int bytesRead = inputStream.EndRead(result);

            //检查是否到达流末尾  
            if (inputStream.Position < inputStream.Length)
            {
                //输出进度  
                string pro = string.Format("{0:0.00}", ((double)inputStream.Position / inputStream.Length) * 100);

                AsyncCheckProgress?.Invoke(new AsyncCheckEventArgs(AsyncCheckState.Checking, pro));

                var output = new byte[buffer.Length];
                //分块计算哈希值  
                hashAlgorithm.TransformBlock(buffer, 0, buffer.Length, output, 0);

                //异步读取下一分块  
                inputStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(AsyncComputeHashCallback), null);
                return;
            }
            else
            {
                //计算最后分块哈希值  
                hashAlgorithm.TransformFinalBlock(buffer, 0, bytesRead);
            }

            string md5 = BitConverter.ToString(hashAlgorithm.Hash).Replace("-", "");

            AsyncCheckProgress?.Invoke(new AsyncCheckEventArgs(AsyncCheckState.Completed, md5));

            //关闭流  
            inputStream.Close();
        }
    }

    internal enum AsyncCheckState
    {
        Completed,
        Checking
    }

    internal class AsyncCheckEventArgs : EventArgs
    {
        internal string Value { get; private set; }

        internal AsyncCheckState State { get; private set; }

        internal AsyncCheckEventArgs(AsyncCheckState state, string value)
        {
            this.Value = value;
            this.State = state;
        }
    }
}
