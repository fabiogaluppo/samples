//Sample provided by Fabio Galuppo  
//May 2018

using System;

public static class Utils
{
	public static unsafe String GetZmqVersion() 
    {
        int[] major = new int[1], minor = new int[1], patch = new int[1];
        ZeroMQ.zmq_version(major, minor, patch);
        return String.Format("{0}.{1}.{2}", major[0], minor[0], patch[0]);
    }
}