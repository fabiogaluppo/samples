//Sample provided by Fabio Galuppo  
//April 2017

using System;
using System.Runtime.InteropServices;

sealed class PinnedBuffer : IDisposable
{
    private GCHandle pinnedObj;

    public PinnedBuffer(Byte[] buffer)
    {
        Buffer = buffer;
        if (Buffer != null)
        {
            Length = buffer.Length;
            if (Length > 0)
            {
                pinnedObj = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                Pointer = pinnedObj.AddrOfPinnedObject();
            }
        }
    }

    public Byte[] Buffer { get; private set; }

    public Int32 Length { get; private set; }
    
    public IntPtr Pointer { get; private set; }

    public static implicit operator IntPtr(PinnedBuffer pinnedBuffer)
    {
        return pinnedBuffer.Pointer;
    }

    public static implicit operator Byte[](PinnedBuffer pinnedBuffer)
    {
        return pinnedBuffer.Buffer;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        FreeResources();
    }

    ~PinnedBuffer()
    {
        FreeResources();
    }

    private void FreeResources()
    {
        if (pinnedObj.IsAllocated)
            pinnedObj.Free();
    }        
}