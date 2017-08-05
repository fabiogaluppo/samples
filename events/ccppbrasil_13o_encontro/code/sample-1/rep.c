//Sample provided by Fabio Galuppo  
//July 2017

//compile:
//clang -I../include -L/usr/local/Cellar/zeromq/4.2.2/lib -lzmq rep.c -o ./bin/rep.exe

//run:
//./bin/rep.exe

#include <zmq.h>
#include <stdio.h>
#include <ctype.h>
#include <string.h>
#include "common.h"

int main(int argc, char* argv[]) {
    char zmq_version_string[64];
    get_zmq_version_string(zmq_version_string);
    printf("ZMQ version = %s\n", zmq_version_string);

    void* zmq_context = zmq_ctx_new();
    void* reply_socket = zmq_socket(zmq_context, ZMQ_REP);
    if (0 == zmq_bind(reply_socket, "tcp://*:60000")) {
        char buffer[64];
        zmq_recv(reply_socket, buffer, sizeof(buffer), 0);
        printf("%s\n", buffer);
        size_t N = strlen(buffer);
        for (size_t i = 0; i < N; ++i)
            buffer[i] = toupper(buffer[i]);
        zmq_send(reply_socket, buffer, N, 0);
    }
    else {
        printf("%s\n", strerror(errno));
    }

    zmq_close(reply_socket);
    zmq_ctx_term(zmq_context);
    return 0;
}