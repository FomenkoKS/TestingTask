using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Testing_Transport_Task
{
    class Program
    {
        private static readonly int maxWeight = 10;
        private static readonly int maxCount = 1000;
        private static readonly int maxLimit = 100;
        private static readonly int maxQuantity = 30;
        private static readonly int maxCost = 20;
        static void Main(string[] args)
        {
            Console.WriteLine("Сгенерировать данные (1) или выбрать файлы(2)?");
            switch (Console.ReadLine())
            {
                case "1":
                    GenData();
                    break;
                case "2":
                    GetData();
                    break;
                default:
                    Console.WriteLine("Выход из программы");
                    break;
            }
            
        }

        private static void GenData()
        {
            Console.WriteLine("Введите количество складов (1-5)");
            int nS = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Введите количество магазинов (1-10)");
            int nM = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Введите количество продукции (1-100)");
            int nP = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Введите количество дней прогноза (1-100)");
            int nD = Int32.Parse(Console.ReadLine());

            
            Random rnd = new Random();
            Dictionary<int, int> products = new Dictionary<int, int>();
            for (int i = 0; i < nP; i++) products.Add(i,rnd.Next(1, maxWeight));
            

            int[,] availability = new int[nS, nP];
            for (int s = 0; s <= availability.GetUpperBound(0); s++)
                for (int p = 0; p <= availability.GetUpperBound(1); p++)
                    availability[s,p]= rnd.Next(0, maxCount);

            Random lrnd = new Random();
            int[,] deliverycosts = new int[nS, nM];
            int[,] limits = new int[nS, nM];
            for (int s = 0; s <= deliverycosts.GetUpperBound(0); s++)
                for (int m = 0; m <= deliverycosts.GetUpperBound(1); m++)
                {
                    //deliverycosts[i, j] = rnd.NextDouble()*5+0.1;
                    deliverycosts[s,m] = rnd.Next(1, maxCost);
                    limits[s,m] = rnd.Next(1, maxLimit);
                }
            
            int[,,] forecast = new int[nD, nP, nM];
            for (int i = 0; i <= forecast.GetUpperBound(0); i++)
                for (int j = 0; j <= forecast.GetUpperBound(1); j++)
                    for (int k = 0; k <= forecast.GetUpperBound(2); k++)
                        forecast[i,j,k] = rnd.Next(1, maxQuantity);

            SolveTransportTask(products, availability, deliverycosts, limits, forecast);
        }

        private static void SolveTransportTask(Dictionary<int, int> products, int[,] availability, int[,] deliverycosts, int[,] limits, int[,,] forecast)
        {
            int s, m;
            int eject;
            int[] lowcostCoords;
            int[,] tempDeliveryCosts;
            int[,] tempLimits;
            List<int[]> delivery= new List<int[]>();
            products = products.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            /* отображение матрицы
            Console.Write('\t');
            for (m = 0; m <= deliverycosts.GetUpperBound(1); m++) Console.Write(m.ToString()+'\t');
            Console.WriteLine("Запасы");

            for (s = 0; s <= deliverycosts.GetUpperBound(0); s++)
            {
                Console.Write(s.ToString() + '\t');
                for (m = 0; m <= deliverycosts.GetUpperBound(1); m++)
                {
                    Console.Write(deliverycosts[s, m].ToString() + '\t');
                }
                Console.WriteLine(availability[s,p].ToString());
            }
            Console.Write("Прогноз" + '\t');
            for (m = 0; m <= deliverycosts.GetUpperBound(1); m++) Console.Write(forecast[d,p,m].ToString() + '\t');


            
            */
            for (int d = 0; d <= forecast.GetUpperBound(0); d++)
            {
                tempLimits = limits.Clone() as int[,];
                for (int p = 0; p < products.Count; p++)
                {
                    tempDeliveryCosts = deliverycosts.Clone() as int[,];
                    lowcostCoords = GetLowCost(tempDeliveryCosts);
                    while (lowcostCoords[0] != -1)
                    {
                        s = lowcostCoords[0];
                        m = lowcostCoords[1];
                        if (forecast[d, p, m] > 0)
                        {
                            eject = new[] { forecast[d, p, m], availability[s, p], tempLimits[s, m] / products[p] }.Min();
                            if (eject > 0)
                            {
                                delivery.Add(new[] { s, m, p, eject });
                            }
                            availability[s, p] -= eject;
                            tempLimits[s, m] -= (products[p] * eject);
                            forecast[d, p, m] -= eject;
                            tempDeliveryCosts[s, m] = -1;
                            if (forecast[d, p, m] == 0) for (int i = 0; i <= tempDeliveryCosts.GetUpperBound(0); i++) tempDeliveryCosts[i, m] = -1;
                            if (availability[s, p] == 0) for (int i = 0; i <= tempDeliveryCosts.GetUpperBound(1); i++) tempDeliveryCosts[s, i] = -1;

                        }
                        else for (s = 0; s <= tempDeliveryCosts.GetUpperBound(0); s++) tempDeliveryCosts[s, m] = -1;
                        lowcostCoords = GetLowCost(tempDeliveryCosts);
                    }
                }
            }

            SaveResult(delivery);
        }

        private static void SaveResult(List<int[]> delivery)
        {
            string csv = "Warehouse, Shop, Product, Count" + '\r'+'\n';
            foreach(int[] a in delivery) csv+= String.Join(',', a.Select(x => x.ToString()))+'\r'+'\n';
            File.WriteAllText("output.csv", csv);
            Console.WriteLine("Файл с планом маршрута поставки сохранен в output.csv");
            Console.ReadKey();
        }

        private static int[] GetLowCost(int[,] deliverycosts)
        {
            int i=-1, j=-1;
            int lowcost = maxCost;
            for (int s = 0; s <= deliverycosts.GetUpperBound(0); s++)
                for (int m = 0; m <= deliverycosts.GetUpperBound(1); m++)
                    if (deliverycosts[s, m] <= lowcost && deliverycosts[s, m]!=-1)
                    {
                        i = s;
                        j = m;
                        lowcost = deliverycosts[i, j];
                    }
            return new[] { i, j };
        }

        private static void GetData()
        {
            Console.WriteLine("Case 3");
        }
        
    }

}
