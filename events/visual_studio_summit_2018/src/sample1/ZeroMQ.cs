//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.12
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class ZeroMQ {
  public static SWIGTYPE_p_void zmq_ctx_new() {
    global::System.IntPtr cPtr = ZeroMQPINVOKE.zmq_ctx_new();
    SWIGTYPE_p_void ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_void(cPtr, false);
    return ret;
  }

  public static int zmq_ctx_term(SWIGTYPE_p_void context) {
    int ret = ZeroMQPINVOKE.zmq_ctx_term(SWIGTYPE_p_void.getCPtr(context));
    return ret;
  }

  public static SWIGTYPE_p_void zmq_socket(SWIGTYPE_p_void arg0, int type) {
    global::System.IntPtr cPtr = ZeroMQPINVOKE.zmq_socket(SWIGTYPE_p_void.getCPtr(arg0), type);
    SWIGTYPE_p_void ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_void(cPtr, false);
    return ret;
  }

  public static int zmq_close(SWIGTYPE_p_void s) {
    int ret = ZeroMQPINVOKE.zmq_close(SWIGTYPE_p_void.getCPtr(s));
    return ret;
  }

  public static int zmq_bind(SWIGTYPE_p_void s, string addr) {
    int ret = ZeroMQPINVOKE.zmq_bind(SWIGTYPE_p_void.getCPtr(s), addr);
    return ret;
  }

  public static int zmq_connect(SWIGTYPE_p_void s, string addr) {
    int ret = ZeroMQPINVOKE.zmq_connect(SWIGTYPE_p_void.getCPtr(s), addr);
    return ret;
  }

  public static int zmq_send(SWIGTYPE_p_void s, global::System.IntPtr buf, uint len, int flags) {
    int ret = ZeroMQPINVOKE.zmq_send(SWIGTYPE_p_void.getCPtr(s), buf, len, flags);
    return ret;
  }

  public static int zmq_recv(SWIGTYPE_p_void s, global::System.IntPtr buf, uint len, int flags) {
    int ret = ZeroMQPINVOKE.zmq_recv(SWIGTYPE_p_void.getCPtr(s), buf, len, flags);
    return ret;
  }

  public unsafe static void zmq_version(int[] major, int[] minor, int[] patch) {
    fixed ( int* swig_ptrTo_major = major ) {
    fixed ( int* swig_ptrTo_minor = minor ) {
    fixed ( int* swig_ptrTo_patch = patch ) {
    {
      ZeroMQPINVOKE.zmq_version((global::System.IntPtr)swig_ptrTo_major, (global::System.IntPtr)swig_ptrTo_minor, (global::System.IntPtr)swig_ptrTo_patch);
    }
    }
    }
    }
  }

  public unsafe static readonly int ZMQ_REQ = ZeroMQPINVOKE.ZMQ_REQ_get();
  public unsafe static readonly int ZMQ_REP = ZeroMQPINVOKE.ZMQ_REP_get();
}
