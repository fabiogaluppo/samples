//Sample provided by Fabio Galuppo 
//June 2015 

//compile: csc /r:System.Core.dll /r:Microsoft.CSharp.dll /r:System.dll /r:System.Net.Http.dll /r:bin\refs\Microsoft.Owin.dll /r:bin\refs\Microsoft.Owin.Host.HttpListener.dll /r:bin\refs\Microsoft.Owin.Hosting.dll /r:bin\refs\Newtonsoft.Json.dll /r:bin\refs\Owin.dll /r:bin\refs\System.Net.Http.Formatting.dll /r:bin\refs\System.Web.Http.dll /r:bin\refs\System.Web.Http.Owin.dll /t:exe /out:bin\MyMicroservice.exe MyMicroserviceProgram.cs
//run: .\bin\MyMicroservice.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Owin;
using Microsoft.Owin.Hosting;
using System.Web.Http;
using System.Net.Http;

namespace MyMicroservice
{
    sealed class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Routes.MapHttpRoute
            (
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        } 
    }

    public sealed class RndController : ApiController
    {
        static readonly Random rnd = new Random(DateTime.Now.Millisecond);

        private static double uniform()
        {
            return rnd.NextDouble();
        }

        private static int uniform(int minInclusive, int maxExclusive)
        {
            return rnd.Next(minInclusive, maxExclusive);
        }

        private static double uniform(double minInclusive, double maxExclusive)
        {
            return minInclusive + uniform() * (maxExclusive - minInclusive);
        }

        private static double gaussian()
        {
            //Box-Muller transform
            double r, x, y;
            do
            {
                x = uniform(-1.0, 1.0);
                y = uniform(-1.0, 1.0);
                r = x * x + y * y;
            }
            while (r >= 1 || r == 0);
            return x * Math.Sqrt(-2.0 * Math.Log(r) / r);
        }

        //Get api/rnd?size={size}&maxExclusive={maxExclusive}&minInclusive={minInclusive}
        public IEnumerable<int> Get(int size, int maxExclusive, int minInclusive)
        {
            if (size > 0)
            {
                var temp = new int[size];
                for (int i = 0; i < size; ++i)
                    temp[i] = uniform(minInclusive, maxExclusive);
                return temp;
            }

            return new int[0];
        }

        //Get api/rnd?size={size}&mean={mean}&stddev={stddev}
        public IEnumerable<double> Get(int size, double mean, double stddev)
        {
            if (size > 0)
            {
                var temp = new double[size];
                for (int i = 0; i < size; ++i)
                    temp[i] = mean + stddev * gaussian();
                return temp;
            }

            return new double[0];
        }
    }

    class Program
    {
        static void TestClient(string baseAddress)
        {
            var client = new HttpClient();
            var response = client.GetAsync(baseAddress + "api/rnd?size=3&maxExclusive=61&minInclusive=1").Result;
            Console.WriteLine(response);
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);

            var response2 = client.GetAsync(baseAddress + "api/rnd?size=3&mean=9.0&stddev=0.2").Result;
            Console.WriteLine(response2);
            Console.WriteLine(response2.Content.ReadAsStringAsync().Result);
        }

        static void Main(string[] args)
        {
            string[] endpoints = { 
                                     "api/rnd?size={size}&maxExclusive={maxExclusive}&minInclusive={minInclusive}", 
                                     "api/rnd?size={size}&mean={mean}&stddev={stddev}",
                                 };

            string baseAddress = "http://localhost:8090/";
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.Title = "Server running at " + baseAddress;
                
                TestClient(baseAddress);
                System.Threading.Thread.Sleep(3000);
                
                Console.Clear();
                Console.WriteLine("Available endpoints in this microservice:");
                
                foreach(var endpoint in endpoints)
                    Console.WriteLine("\t" + baseAddress + endpoint);
                
                Console.WriteLine("[Enter to exit...]");
                Console.ReadLine();
            }
        }
    }
}
