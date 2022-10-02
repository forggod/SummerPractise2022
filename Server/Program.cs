using System;
using System.Threading;

namespace Project_2022_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string ip = "127.0.0.1";
            const int port = 7070;


            ThreadPool.SetMinThreads(2, 2);
            ThreadPool.SetMaxThreads(10, 10);     // Макс кол-во обычных и асинхронно работающих потоков

            Server server = new Server(ip, port);
            server.Start();

            string cons;
            while (true)
            {
                cons = Console.ReadLine();
                if (cons == "stop")
                {
                    server.Stop();
                }
            }
        }
    }
}
