//Sample provided by Fabio Galuppo  
//May 2018

//swig -csharp zmq.i
//clang -shared -undefined dynamic_lookup -L/usr/local/Cellar/zeromq/4.2.2/lib -lzmq -o ZeroMQ zmq_wrap.c

%module ZeroMQ
%{
#include <zmq.h>
%}

%include "arrays_csharp.i"

extern void* zmq_ctx_new();
extern int zmq_ctx_term(void *context);
extern void *zmq_socket(void *, int type);
extern int zmq_close(void *s);
extern int zmq_bind(void *s, const char *addr);
extern int zmq_connect(void *s, const char *addr);

%apply void *VOID_INT_PTR { void* buf }
extern int zmq_send(void *s, const void *buf, size_t len, int flags);
extern int zmq_recv(void *s, void *buf, size_t len, int flags);

%csmethodmodifiers "public unsafe";
%apply int FIXED[] { int* major }
%apply int FIXED[] { int* minor }
%apply int FIXED[] { int* patch }
extern void zmq_version (int *major, int *minor, int *patch);

%constant int ZMQ_REQ = ZMQ_REQ;
%constant int ZMQ_REP = ZMQ_REP;