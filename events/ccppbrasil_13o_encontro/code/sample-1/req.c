//Sample provided by Fabio Galuppo  
//April 2017

//compile:
//clang -I../include -L/usr/local/Cellar/zeromq/4.2.2/lib -lzmq req.c -o ./bin/req.exe

//run:
//./bin/req.exe

#include <zmq.h>
#include <stdio.h>
#include <string.h>
#include "common.h"

int main(int argc, char* argv[]) {
    char zmq_version_string[64];
    get_zmq_version_string(zmq_version_string);
    printf("ZMQ version = %s\n", zmq_version_string);

    void* zmq_context = zmq_ctx_new();
    void* request_socket = zmq_socket(zmq_context, ZMQ_REQ);
    zmq_connect(request_socket, "tcp://localhost:60000");
    
    char* msg = "Hello, World!";
    if (argc > 1)
        msg = argv[1];
    
    zmq_send(request_socket, msg, strlen(msg), 0);

    char buffer[64];
    zmq_recv(request_socket, buffer, sizeof(buffer), 0);
    printf("%s\n", buffer);

    zmq_close(request_socket);
    zmq_ctx_term(zmq_context);
    return 0;
}