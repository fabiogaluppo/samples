using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace CommonLib
{
    public interface ISharedMemoryReader
    {
        int Read();
        int Read(byte[] buffer, int index, int count);
        int Read(char[] buffer, int index, int count);
        bool ReadBoolean();
        byte ReadByte();
        byte[] ReadBytes(int count);
        char ReadChar();
        char[] ReadChars(int count);
        decimal ReadDecimal();
        double ReadDouble();
        short ReadInt16();
        int ReadInt32();
        long ReadInt64();
        sbyte ReadSByte();
        float ReadSingle();
        string ReadString();
        ushort ReadUInt16();
        uint ReadUInt32();
        ulong ReadUInt64();
    }

    public sealed class SharedMemoryReader : ISharedMemoryReader, IDisposable
    {
        private readonly MemoryMappedViewStream MMVS_;
        private readonly BinaryReader BinReader_;
        
        internal SharedMemoryReader(SharedMemory source, MemoryMappedViewStream mmvs)
        {
            MMVS_ = mmvs;
            BinReader_ = new BinaryReader(MMVS_);
            Source = source;
        }

        private void FreeResources()
        {
            BinReader_.Dispose();            
            MMVS_.Dispose();
        }

        public SharedMemory Source { get; private set; }
        
        public long Offset { get { return MMVS_.Position; } }
        
        public long Size { get { return MMVS_.Length; } }

        #region "Dispose pattern"
        private int Disposed_ = 0;

        ~SharedMemoryReader()
        {
            FreeResources();
        }

        public void Dispose()
        {
            if (0 == Interlocked.CompareExchange(ref Disposed_, 1, 0))
            {
                GC.SuppressFinalize(this);
                FreeResources();
            }
        }
        #endregion

        #region "ISharedMemoryReader implementation"
        public int Read()
        {
            return BinReader_.Read();
        }

        public int Read(byte[] buffer, int index, int count)
        {
            return BinReader_.Read(buffer, index, count);
        }

        public int Read(char[] buffer, int index, int count)
        {
            return BinReader_.Read(buffer, index, count);
        }

        public bool ReadBoolean()
        {
            return BinReader_.ReadBoolean();
        }

        public byte ReadByte()
        {
            return BinReader_.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            return BinReader_.ReadBytes(count);
        }

        public char ReadChar()
        {
            return BinReader_.ReadChar();
        }

        public char[] ReadChars(int count)
        {
            return BinReader_.ReadChars(count);
        }

        public decimal ReadDecimal()
        {
            return BinReader_.ReadDecimal();
        }

        public double ReadDouble()
        {
            return BinReader_.ReadDouble();
        }

        public short ReadInt16()
        {
            return BinReader_.ReadInt16();
        }

        public int ReadInt32()
        {
            return BinReader_.ReadInt32();
        }

        public long ReadInt64()
        {
            return BinReader_.ReadInt64();
        }

        public sbyte ReadSByte()
        {
            return BinReader_.ReadSByte();
        }

        public float ReadSingle()
        {
            return BinReader_.ReadSingle();
        }

        public string ReadString()
        {
            return BinReader_.ReadString();
        }

        public ushort ReadUInt16()
        {
            return BinReader_.ReadUInt16();
        }

        public uint ReadUInt32()
        {
            return BinReader_.ReadUInt32();
        }

        public ulong ReadUInt64()
        {
            return BinReader_.ReadUInt64();
        }
        #endregion
    }

    public interface ISharedMemoryWriter
    {
        void Write(bool value);
        void Write(byte value);
        void Write(byte[] buffer);
        void Write(char ch);
        void Write(char[] chars);
        void Write(decimal value);
        void Write(double value);
        void Write(float value);
        void Write(int value);
        void Write(long value);
        void Write(sbyte value);
        void Write(short value);
        void Write(string value);
        void Write(uint value);
        void Write(ulong value);
        void Write(ushort value);
        void Write(byte[] buffer, int index, int count);
        void Write(char[] chars, int index, int count);
    }

    public sealed class SharedMemoryWriter : ISharedMemoryWriter, IDisposable
    {
        private readonly MemoryMappedViewStream MMVS_;
        private readonly BinaryWriter BinWriter_;        

        internal SharedMemoryWriter(SharedMemory source, MemoryMappedViewStream mmvs)
        {
            MMVS_ = mmvs;
            BinWriter_ = new BinaryWriter(MMVS_);
            Source = source;
        }

        public long Offset { get { return MMVS_.PointerOffset; } }

        public long Size { get { return MMVS_.Length;  } }

        public SharedMemory Source { get; private set; }

        private void FreeResources()
        {
            BinWriter_.Dispose();            
            MMVS_.Dispose();
        }

        #region "Dispose pattern"
        private int Disposed_ = 0;

        ~SharedMemoryWriter()
        {
            FreeResources();
        }

        public void Dispose()
        {
            if (0 == Interlocked.CompareExchange(ref Disposed_, 1, 0))
            {
                GC.SuppressFinalize(this);
                FreeResources();
            }
        }
        #endregion

        #region "ISharedMemoryWriter implementation"
        public void Write(bool value)
        {
            BinWriter_.Write(value);
        }

        public void Write(byte value)
        {
            BinWriter_.Write(value);
        }

        public void Write(byte[] buffer)
        {
            BinWriter_.Write(buffer);
        }

        public void Write(char ch)
        {
            BinWriter_.Write(ch);
        }

        public void Write(char[] chars)
        {
            BinWriter_.Write(chars);
        }

        public void Write(decimal value)
        {
            BinWriter_.Write(value);
        }

        public void Write(double value)
        {
            BinWriter_.Write(value);
        }

        public void Write(float value)
        {
            BinWriter_.Write(value);
        }

        public void Write(int value)
        {
            BinWriter_.Write(value);
        }

        public void Write(long value)
        {
            BinWriter_.Write(value);
        }

        public void Write(sbyte value)
        {
            BinWriter_.Write(value);
        }

        public void Write(short value)
        {
            BinWriter_.Write(value);
        }

        public void Write(string value)
        {
            BinWriter_.Write(value);
        }

        public void Write(uint value)
        {
            BinWriter_.Write(value);
        }

        public void Write(ulong value)
        {
            BinWriter_.Write(value);
        }

        public void Write(ushort value)
        {
            BinWriter_.Write(value);
        }

        public void Write(byte[] buffer, int index, int count)
        {
            BinWriter_.Write(buffer, index, count);
        }

        public void Write(char[] chars, int index, int count)
        {
            BinWriter_.Write(chars, index, count);
        }
        #endregion
    }

    public sealed class SharedMemory : IDisposable
    {
        private readonly MemoryMappedFile MMF_;
        
        private SharedMemory(string name, long maxSize)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (maxSize < 0)
                throw new ArgumentException("maxSize");

            Name = name;            
            MMF_ = MemoryMappedFile.CreateNew(Name, maxSize, MemoryMappedFileAccess.ReadWrite);
        }

        private SharedMemory(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Name = name;
            MMF_ = MemoryMappedFile.OpenExisting(name, MemoryMappedFileRights.ReadWrite);
        }

        public string Name { get;  private set; }

        public SharedMemoryReader AsReader(long offset = 0L, long size = 0L)
        {
            return new SharedMemoryReader(this, MMF_.CreateViewStream(offset, size, MemoryMappedFileAccess.Read));
        }
        public SharedMemoryWriter AsWriter(long offset = 0L, long size = 0L)
        {
            return new SharedMemoryWriter(this, MMF_.CreateViewStream(offset, size, MemoryMappedFileAccess.Write));
        }

        public static SharedMemory Create(string name, long maxSize)
        {
            return new SharedMemory(name, maxSize);
        }

        public static SharedMemory Open(string name)
        {
            return new SharedMemory(name);        
        }
        private void FreeResources()
        {
            MMF_.Dispose();
        }

        #region "Dispose pattern"
        private int Disposed_ = 0;

        ~SharedMemory()
        {
            FreeResources();
        }

        public void Dispose()
        {
            if (0 == Interlocked.CompareExchange(ref Disposed_, 1, 0))
            {
                GC.SuppressFinalize(this);
                FreeResources();
            }
        }
        #endregion
    }
}
