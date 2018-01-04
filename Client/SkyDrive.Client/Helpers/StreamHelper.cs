using System;
using System.IO;

namespace SkyDrive.Client
{
    public class StreamHelper
    {
        private byte[] Data;

        private const int ByteCount = 1024;

        public FileStream Stream { get; private set; }

        public BinaryReader Reader { get; private set; }

        public int CurrentPositon { get; private set; }

        public StreamHelper(string path, int position)
        {
            Stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            Reader = new BinaryReader(Stream);
            CurrentPositon = position;

            if (position > 0) { Stream.Seek(CurrentPositon, SeekOrigin.Current); }
        }

        public byte[] GetNextChunk()
        {
            long _length = Stream.Length;

            if (CurrentPositon > _length) { return null; }

            if (CurrentPositon + ByteCount > _length)
            {
                Data = new byte[_length - CurrentPositon];
                Reader.Read(Data, 0, Convert.ToInt16(_length - CurrentPositon));
            }
            else
            {
                Data = new byte[ByteCount];
                Reader.Read(Data, 0, ByteCount);
            }

            CurrentPositon += ByteCount;

            return Data;
        }
    }
}
