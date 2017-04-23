//Sample provided by Fabio Galuppo  
//April 2017

using System;
using System.Runtime.InteropServices;

static unsafe class ZMQ
{
    public const Int32 ZMQ_PAIR = 0;
    public const Int32 ZMQ_PUB = 1;
    public const Int32 ZMQ_SUB = 2;
    public const Int32 ZMQ_REQ = 3;
    public const Int32 ZMQ_REP = 4;
    public const Int32 ZMQ_DEALER = 5;
    public const Int32 ZMQ_ROUTER = 6;
    public const Int32 ZMQ_PULL = 7;
    public const Int32 ZMQ_PUSH = 8;
    public const Int32 ZMQ_XPUB = 9;
    public const Int32 ZMQ_XSUB = 10;
    public const Int32 ZMQ_STREAM = 11;

    private const string DllName = "/Users/fabiogaluppo/zmq/samples/refs/libzmq.4.2.3.dylib";
    
    [DllImport(DllName, EntryPoint = "zmq_version", CallingConvention = CallingConvention.Cdecl)]
    public static extern void zmq_version(out Int32 major, out Int32 minor, out Int32 patch);

    [DllImport(DllName, EntryPoint = "zmq_ctx_new", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr zmq_ctx_new();

    [DllImport(DllName, EntryPoint = "zmq_ctx_term", CallingConvention = CallingConvention.Cdecl)]
	public static extern Int32 zmq_ctx_term(IntPtr context);

    [DllImport(DllName, EntryPoint = "zmq_socket", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr zmq_socket(IntPtr context, Int32 type);

    [DllImport(DllName, EntryPoint = "zmq_close", CallingConvention = CallingConvention.Cdecl)]
	public static extern Int32 zmq_close(IntPtr socket);

    [DllImport(DllName, EntryPoint = "zmq_bind", CallingConvention = CallingConvention.Cdecl)]
	public static extern Int32 zmq_bind(IntPtr socket, IntPtr endpoint);

    [DllImport(DllName, EntryPoint = "zmq_connect", CallingConvention = CallingConvention.Cdecl)]
	public static extern Int32 zmq_connect(IntPtr socket, IntPtr endpoint);

    [DllImport(DllName, EntryPoint = "zmq_send", CallingConvention = CallingConvention.Cdecl)]
	public static extern Int32 zmq_send(IntPtr socket, IntPtr buf, Int32 len, Int32 flags);

    [DllImport(DllName, EntryPoint = "zmq_recv", CallingConvention = CallingConvention.Cdecl)]
	public static extern Int32 zmq_recv(IntPtr socket, IntPtr buf, Int32 len, Int32 flags);

    [DllImport(DllName, EntryPoint = "zmq_setsockopt", CallingConvention = CallingConvention.Cdecl)]
	public static extern Int32 zmq_setsockopt(IntPtr socket, Int32 option_name, IntPtr option_value, Int32 option_len);

    [DllImport(DllName, EntryPoint = "zmq_getsockopt", CallingConvention = CallingConvention.Cdecl)]
	public static extern Int32 zmq_getsockopt(IntPtr socket, Int32 option_name, IntPtr option_value, IntPtr option_len);

    public static String GetVersionString()
    {
        Int32 major, minor, patch;
        ZMQ.zmq_version(out major, out minor, out patch);
        return String.Format($"{major}.{minor}.{patch}");
    }
} 