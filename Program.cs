using System;
using System.Net.NetworkInformation;
using System.Threading;

namespace Travailleurs
{
    class Program
    {

        private static Semaphore screwDriver1;
        private static Semaphore screwDriver2;
        private static Semaphore wrench1;
        private static Semaphore wrench2;

        private static int crafted = 0;

        private static string[] threadState = new string[5];

        static void Main(string[] args)
        {
            Console.WriteLine("Demarage");

            screwDriver1 = new Semaphore(0, 1); //Worker 1
            screwDriver2 = new Semaphore(0, 1); //Worker 3

            wrench1 = new Semaphore(0, 1); //Worker 2
            wrench2 = new Semaphore(0, 1); //Worker 4



            for (int i = 1; i < 5; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(Worker));

                t.Start(i);
            }

            Thread t2 = new Thread(Foreman);
            t2.Start();

            Thread.Sleep(500);

            screwDriver1.Release(1);
            screwDriver2.Release(1);
            wrench1.Release(1);
            wrench2.Release(1);

            

            Console.ReadKey();
        }

        private static void Foreman()
        {
            while (true)
            {
                Thread.Sleep(1000);
                if (threadState[1] == "waiting" && threadState[2] == "waiting" && threadState[3] == "waiting" && threadState[4] == "waiting")
                {
                    Console.WriteLine("It seems that everyworker are waiting for tools...");
                    Thread.Sleep(1000);
                    if (threadState[1] == "waiting" && threadState[2] == "waiting" && threadState[3] == "waiting" && threadState[4] == "waiting")
                    {
                        Console.WriteLine("The foreman is releasing some tools to unlock the situation");
                        var rand = new Random();
                        var nb = rand.Next(2);

                        if (nb == 1)
                        {
                            screwDriver1.Release(1);
                            screwDriver2.Release(1);
                        }
                        else
                        {
                            wrench1.Release(1);
                            wrench2.Release(1);
                        }
                    }
                };
            }
        }
        private static void Worker(object num)
        {
            while (true)
            {

                Console.WriteLine("Worker " + num + " is waiting for tools");
                threadState[(int) num] = "waiting";

                switch (num)
                {
                    case 1:
                        wrench2.WaitOne();
                        screwDriver1.WaitOne();
                        break;
                    case 2:
                        screwDriver1.WaitOne();
                        wrench1.WaitOne();
                        break;
                    case 3:
                        screwDriver2.WaitOne();
                        wrench1.WaitOne();
                        break;
                    case 4:
                        screwDriver2.WaitOne();
                        wrench2.WaitOne();
                        break;
                }

                Console.WriteLine("Worker " + num +  " get tools and started assambling for 4s...");
                threadState[(int)num] = "assambling";
                Thread.Sleep(4000);

                switch (num)
                {
                    case 1:
                        wrench2.Release(1);
                        screwDriver1.Release(1);
                        break;
                    case 2:
                        screwDriver1.Release(1);
                        wrench1.Release(1);
                        break;
                    case 3:
                        screwDriver2.Release(1);
                        wrench1.Release(1);
                        break;
                    case 4:
                        wrench2.Release(1);
                        screwDriver2.Release(1);
                        break;
                }

                Thread.Sleep(500);
                Console.WriteLine("Worker " + num + " finished an release his tools.");

                Action<object?> inc = (obj) =>
                {
                    crafted++;
                    Console.WriteLine("Object crafted: " + crafted);
                };

                lock(inc)
                {
                    inc(null);
                };

            }
        }
    }
}

