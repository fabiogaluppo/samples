//Sample provided by Fabio Galuppo  
//May 2018

using System;
using System.IO;

public static class Utils
{
	public static unsafe String GetZmqVersion() 
    {
        int[] major = new int[1], minor = new int[1], patch = new int[1];
        ZeroMQ.zmq_version(major, minor, patch);
        return String.Format("{0}.{1}.{2}", major[0], minor[0], patch[0]);
    }
}

//Inspired by https://github.com/kevin-wayne/algs4/blob/master/src/main/java/edu/princeton/cs/algs4/StdRandom.java
static class RandomExtensions
{
    public static double Uniform(this Random rnd)
    {
        return rnd.NextDouble();
    }

    public static int Uniform(this Random rnd, int n)
    {
        return rnd.Next(n);
    }

    public static int Uniform(this Random rnd, int a, int b)
    {
        return a + Uniform(rnd, b - a);
    }

    public static double Uniform(this Random rnd, double a, double b)
    {
        return a + Uniform(rnd) * (b - a);
    }

    public static double Gaussian(this Random rnd)
    {
        double r, x, y;

        do
        {
            x = Uniform(rnd, -1.0, 1.0);
            y = Uniform(rnd, -1.0, 1.0);
            r = x * x + y * y;
        }
        while (r >= 1 || r == 0);

        return x * Math.Sqrt(-2 * Math.Log(r) / r);
    }

    public static double Gaussian(this Random rnd, double mu, double sigma)
    {
        return mu + sigma * Gaussian(rnd);
    }

    public static double Cauchy(this Random rnd)
    {
        return Math.Tan(Math.PI * (Uniform(rnd) - 0.5));
    }
}

static class MemoryStreamExtensions
{
    public static MemoryStream Append(this MemoryStream ms, byte x)
    {
        ms.Append(new byte[]{ x });
        return ms;
    }

    public static MemoryStream Append(this MemoryStream ms, byte[] xs)
    {
        ms.Write(xs, 0, xs.Length);
        return ms;
    }
}
