//Sample provided by Fabio Galuppo  
//April 2017

#ifndef common_h
#define common_h

#include <zmq.h>
#include <stdio.h>

void get_zmq_version_string(char* zmq_version_string) {
    int major, minor, patch;
    zmq_version(&major, &minor, &patch);
    sprintf(zmq_version_string, "%d.%d.%d", major, minor, patch);
}

#endif /* common_h */