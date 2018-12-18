using System;

namespace Testing_Transport_Task
{
    class Program
    {
        private static readonly int maxWeight = 10;
        private static readonly int maxCount = 1000;
        private static readonly int maxLimit = 30;
        private static readonly int maxQuantity = 30;
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
            int[] products = new int[nP];
            for (int i = 0; i < products.Length; i++) products[i] = rnd.Next(1, maxWeight);

            int[,] availability = new int[nS, nP];
            for (int i = 0; i <= availability.GetUpperBound(0); i++)
                for (int j = 0; j <= availability.GetUpperBound(1); j++)
                    availability[i,j]= rnd.Next(0, maxCount);

            Random lrnd = new Random();
            double[,,] pathes = new double[nS, nM,2];
            for (int i = 0; i <= pathes.GetUpperBound(0); i++)
                for (int j = 0; j <= pathes.GetUpperBound(1); j++)
                {

                    pathes[i, j,0] = rnd.NextDouble()*5+0.1;
                    //pathes[i,j,0] = rnd.Next(1, 20);
                    pathes[i,j,1] = rnd.Next(1, maxLimit);
                }
            
            float[,,] forecast = new float[nD, nP, nM];
            for (int i = 0; i <= forecast.GetUpperBound(0); i++)
                for (int j = 0; j <= forecast.GetUpperBound(1); j++)
                    for (int k = 0; k <= forecast.GetUpperBound(2); k++)
                        forecast[i,j,k] = rnd.Next(1, maxQuantity);

            SolveTransportTask(products, availability, pathes, forecast);
        }

        private static void SolveTransportTask(int[] products, int[,] availability, double[,,] pathes, float[,,] forecast)
        {
            int p = 0;
            int d = 0;
            double cost;
            Console.Write('\t');
            for (int m = 0; m <= pathes.GetUpperBound(1); m++) Console.Write(m.ToString()+'\t');
            Console.WriteLine("Запасы");

            for (int s = 0; s <= pathes.GetUpperBound(0); s++)
            {
                Console.Write(s.ToString() + '\t');
                for (int m = 0; m <= pathes.GetUpperBound(1); m++)
                {
                    cost = Math.Round(pathes[s, m, 0] * products[p],2);
                    Console.Write(cost.ToString() + '\t');
                }
                Console.WriteLine(availability[s,p].ToString());
            }
            Console.Write("Прогноз" + '\t');
            for (int m = 0; m <= pathes.GetUpperBound(1); m++) Console.Write(forecast[d,p,m].ToString() + '\t');
        }

        private static void GetData()
        {
            Console.WriteLine("Case 3");
        }
        
    }

}
