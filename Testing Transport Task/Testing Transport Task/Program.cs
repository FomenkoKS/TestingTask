using System;

namespace Testing_Transport_Task
{
    class Program
    {
        private static readonly int maxWeight = 10;
        private static readonly int maxCount = 10;
        private static readonly int maxLimit = 30;
        private static readonly int maxQuantity = 30;
        static void Main(string[] args)
        {
            Console.WriteLine("Сгенерировать данные (1) или выбрать файлы(2)?");
            switch (Console.ReadLine())
            {
                case "1":
                    genData();
                    Console.ReadLine();
                    break;
                case "2":
                    getData();
                    Console.ReadLine();
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
            
        }

        private static void genData()
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
            int[] Products = new int[nP];
            for (int i = 0; i < Products.Length; i++) Products[i] = rnd.Next(1, maxWeight);

            int[,] Availability = new int[nS, nP];
            for (int i = 0; i <= Availability.GetUpperBound(0); i++)
                for (int j = 0; j <= Availability.GetUpperBound(1); j++)
                    Availability[i,j]= rnd.Next(0, maxCount);

            Random lrnd = new Random();
            double[,,] Pathes = new double[nS, nM,2];
            for (int i = 0; i <= Pathes.GetUpperBound(0); i++)
                for (int j = 0; j <= Pathes.GetUpperBound(1); j++)
                {
                    
                    Pathes[i, j,0] = rnd.NextDouble()*5+0.1;
                    Pathes[i, j,1] = rnd.Next(1, maxLimit);
                }
            
            float[,,] Forecast = new float[nM, nP, nD];
            for (int i = 0; i <= Forecast.GetUpperBound(0); i++)
                for (int j = 0; j <= Forecast.GetUpperBound(1); j++)
                    for (int k = 0; k <= Forecast.GetUpperBound(2); k++)
                        Forecast[i,j,k] = rnd.Next(1, maxQuantity);

        }

        private static void getData()
        {
            Console.WriteLine("Case 3");
        }
    }

}
