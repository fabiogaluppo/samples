//Sample provided by Fabio Galuppo
//June 2015

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib;

namespace HostA
{
    class ProgramHostA
    {
        static void Main(string[] args)
        {
            Console.Title = "HostA";
            SharedMemory sm = SharedMemory.Create("SM1", 1024);
            using(var w = sm.AsWriter())
            {
                w.Write(1);
                w.Write(2);
                w.Write(3);
                w.Write(new byte[5] { 0x4, 0x5, 0x6, 0x7, 0x8 });
            }
            Console.WriteLine("Values written to shared memory = {0}", sm.Name);
            Console.ReadLine();
            SharedMemory sm2 = SharedMemory.Create("SM2", 2L * 1024);
            using (var w = sm2.AsWriter(offset: 0L, size: 1L * 1024))
            {
                var buffer1 = new byte[1 * 1024];
                for (int i = 0; i < buffer1.Length; i += 2)
                {
                    buffer1[i] = 0xA;
                    buffer1[i + 1] = 0xB;
                }
                w.Write(buffer1);
                Console.WriteLine("Values written to shared memory = {0} offset = {1} size = {2}", w.Source.Name, w.Offset, w.Size);
            }
            Console.ReadLine();
            using (var w = sm2.AsWriter(offset: 1L * 1024, size: 1L * 1024))
            {
                var buffer2 = new byte[1 * 1024];
                for (int i = 0; i < buffer2.Length; i += 2)
                {
                    buffer2[i] = 0xD;
                    buffer2[i + 1] = 0xE;
                }
                w.Write(buffer2);
                Console.WriteLine("Values written to shared memory = {0} offset = {1} size = {2}", w.Source.Name, w.Offset, w.Size);
            }
            Console.ReadLine();
            using (var w = sm2.AsWriter(offset: 0L, size: 1L * 1024))
            {
                var buffer1 = new byte[1 * 1024];
                for (int i = 0; i < buffer1.Length; i += 2)
                {
                    buffer1[i] = 0xC;
                    buffer1[i + 1] = 0xF;
                }
                w.Write(buffer1);
                Console.WriteLine("Values written to shared memory = {0} offset = {1} size = {2}", w.Source.Name, w.Offset, w.Size);
            }
            Console.ReadLine();
        }
    }
}
