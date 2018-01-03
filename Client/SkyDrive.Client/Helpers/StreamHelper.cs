using System;
using System.IO;

namespace SkyDrive.Client
{
    internal class StreamHelper
    {
        private const int ByteCount = 1024;

        internal FileStream Stream { get; private set; }

        internal BinaryReader Reader { get; private set; }

        internal int CurrentPositon { get; private set; }

        internal StreamHelper(string path)
        {
            Stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            Reader = new BinaryReader(Stream);
        }

        public byte[] GetNextChunk()
        {
            long _length = Stream.Length;

            if (CurrentPositon > _length) { return null; }

            if (CurrentPositon > 0) { Stream.Seek(CurrentPositon, SeekOrigin.Current); }

            byte[] data;

            if (CurrentPositon + ByteCount > _length)
            {
                data = new byte[_length - CurrentPositon];
                Reader.Read(data, 0, Convert.ToInt16(_length - CurrentPositon));
            }
            else
            {
                data = new byte[ByteCount];
                Reader.Read(data, 0, ByteCount);
            }

            CurrentPositon += ByteCount;

            return data;
        }
    }
}
