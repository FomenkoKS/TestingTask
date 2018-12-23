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
            int nS=0, nM=0, nP=0, nD=0;
            Boolean flag = true;
            //Запрос информации у пользователя
            while (flag)
            {
                try
                {
                    Console.WriteLine("Введите количество складов (1-5)");
                    nS = Int32.Parse(Console.ReadLine());
                    while (nS < 1 || nS > 5) nS = Int32.Parse(Console.ReadLine()); //Защита от дурака

                    Console.WriteLine("Введите количество магазинов (1-10)");
                    nM = Int32.Parse(Console.ReadLine());
                    while (nM < 1 || nM > 10) nM = Int32.Parse(Console.ReadLine());

                    Console.WriteLine("Введите количество продукции (1-100)");
                    nP = Int32.Parse(Console.ReadLine());
                    while (nP < 1 || nP > 100) nP = Int32.Parse(Console.ReadLine());

                    Console.WriteLine("Введите количество дней прогноза (1-100)");
                    nD = Int32.Parse(Console.ReadLine());
                    while (nD < 1 || nD > 100) nD = Int32.Parse(Console.ReadLine());

                    flag = false;
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Похоже вы ввели не число, попробуйте ввести данные заново");
                }
            }
            

            //Генератор
            Random rnd = new Random();
            //Объявление необходимых массивов и словарей 
            Dictionary<int, int> products = new Dictionary<int, int>();
            int[,] availability = new int[nS, nP];
            int[,] deliverycosts = new int[nS, nM];
            int[,] limits = new int[nS, nM];
            int[,,] forecast = new int[nD, nP, nM];

            //Генерация продуктов
            for (int i = 0; i < nP; i++) products.Add(i,rnd.Next(1, maxWeight));

            // Загрузка тестовых данных в файл 
            /*string csv = "";
            foreach (KeyValuePair<int, int> entry in products)
            {
                csv += entry.Key.ToString() + ',' + entry.Value.ToString() + '\r' + '\n';
            }
            File.WriteAllText("products.csv", csv);
            csv = "";
            */

            //Генерация доступности продукции на складах
            for (int s = 0; s <= availability.GetUpperBound(0); s++)
                for (int p = 0; p <= availability.GetUpperBound(1); p++)
                {
                    availability[s, p] = rnd.Next(0, maxCount);
                    //csv += s.ToString() + ',' + p.ToString() + ',' + availability[s, p].ToString() + '\r' + '\n';
                }
            //File.WriteAllText("availability.csv", csv);
            //csv = "";
            
            //Генерация весов путей и лимитов по загрузке
            for (int s = 0; s <= deliverycosts.GetUpperBound(0); s++)
                for (int m = 0; m <= deliverycosts.GetUpperBound(1); m++)
                {
                    //deliverycosts[i, j] = rnd.NextDouble()*5+0.1;
                    deliverycosts[s,m] = rnd.Next(1, maxCost);
                    limits[s,m] = rnd.Next(1, maxLimit);
                    //csv += s.ToString() + ',' + m.ToString() + ',' + deliverycosts[s, m].ToString() + ',' + limits[s, m].ToString() + '\r' + '\n';
                }
            //File.WriteAllText("pathes.csv", csv);
            //csv = "";

            //Генерация прогнозов
            for (int i = 0; i <= forecast.GetUpperBound(0); i++)
                for (int j = 0; j <= forecast.GetUpperBound(1); j++)
                    for (int k = 0; k <= forecast.GetUpperBound(2); k++)
                    {
                        forecast[i, j, k] = rnd.Next(1, maxQuantity);
                        //csv += i.ToString() + ',' + j.ToString() + ',' + k.ToString() + ',' + forecast[i, j, k].ToString() + '\r' + '\n';
                    }
            //File.WriteAllText("forecast.csv", csv);
            
            SolveTransportTask(products, availability, deliverycosts, limits, forecast);
        }

        //Решение транспортной задачи методом наименьшей стоимости
        private static void SolveTransportTask(Dictionary<int, int> products, int[,] availability, int[,] deliverycosts, int[,] limits, int[,,] forecast)
        {

            //Объявление переменных,..
            int s, m;
            int eject;
            int[] lowcostCoords;
            //... временных копий,..
            int[,] tempDeliveryCosts;
            int[,] tempLimits;
            //... и исходного маршрутного листа
            List<int[]> delivery= new List<int[]>();
            //сортируем продукты по весу, лёгкие отправляем первыми
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

            //цикл по дням
            for (int d = 0; d <= forecast.GetUpperBound(0); d++)
            {
                //лимиты сбрасываем ежедневно
                tempLimits = limits.Clone() as int[,];
                //цикл по продуктам
                for (int p = 0; p < products.Count; p++)
                {
                    //сброс стоимости пути для каждого продукта
                    tempDeliveryCosts = deliverycosts.Clone() as int[,];
                    //ищем наименьшую стоимость
                    lowcostCoords = GetLowCost(tempDeliveryCosts);
                    //пока не обойдём все стоимости/не удовлетворим потребности магазинов/не очистим склады/не уткнёмся в лимиты -- очищаем и удовлетворяем
                    while (lowcostCoords[0] != -1)
                    {
                        s = lowcostCoords[0];
                        m = lowcostCoords[1];
                        if (forecast[d, p, m] > 0)
                        {
                            eject = new[] { forecast[d, p, m], availability[s, p], tempLimits[s, m] / products[p] }.Min();
                            if (eject > 0)
                            {
                                delivery.Add(new[] { d, s, m, p, eject });
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

        //сохранение маршрутного листа в файл
        private static void SaveResult(List<int[]> delivery)
        {
            string csv = "Day, Warehouse, Shop, Product, Count" + '\r'+'\n';
            foreach(int[] a in delivery) csv+= String.Join(',', a.Select(x => x.ToString()))+'\r'+'\n';
            File.WriteAllText("output.csv", csv);
            Console.WriteLine("Файл с планом маршрута поставки сохранен в output.csv");
            Console.ReadKey();
        }

        //функция поиска наименьшей цены в матрице транспортной задачи
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
            string path;
            //Объявление необходимых массивов и словарей 
            Dictionary<int, int> products = new Dictionary<int, int>();
            List<int[]> csvLines = new List<int[]>();

            //Выявление количества параметров
            int nS = 0, nM = 0, nP = 0, nD = 0;

            //Загрузка в массив списка продуктов
            Console.WriteLine("Укажите путь до csv файла со списком продукции. Структура файла по столбцам: id продукта (0-99), вес. Или нажмите Enter для загрузки products.csv");
            path = ListenPath("products.csv");
            foreach (string line in File.ReadAllLines(path))
            {
                var tmp = Array.ConvertAll(line.Split(','), item => Int32.Parse(item));
                products.Add(tmp[0], tmp[1]);
            }

            //Загрузка в массив наличия продуктов на складе.
            Console.WriteLine("Укажите путь до csv файла наличия продукции на складе. Структура файла по столбцам: id склада (0-4), id продукта (0-99), количество. Или нажмите Enter для загрузки availability.csv");
            path = ListenPath("availability.csv");
            foreach (string line in File.ReadAllLines(path))
            {
                var tmp = Array.ConvertAll(line.Split(','), item => Int32.Parse(item));
                if (tmp[0] > nS) nS = tmp[0];
                if (tmp[1] > nS) nP = tmp[1];
                csvLines.Add(tmp);
            }
            int[,] availability = new int[nS+1, nP+1];
            foreach (int[] line in csvLines) availability[line[0], line[1]] = line[2];


            //Загрузка в массивы стоимости путей и лимитов доставки.
            csvLines = new List<int[]>();
            Console.WriteLine("Укажите путь до csv файла со списком маршрутов. Структура файла по столбцам: id склада (0-4), id магазина (0-9), стоимость, лимит перевозки. Или нажмите Enter для загрузки pathes.csv");
            path = ListenPath("pathes.csv");
            foreach (string line in File.ReadAllLines(path))
            {
                var tmp = Array.ConvertAll(line.Split(','), item => Int32.Parse(item));
                if (tmp[0] > nS) nS = tmp[0];
                if (tmp[1] > nM) nM = tmp[1];
                csvLines.Add(tmp);
            }
            int[,] deliverycosts = new int[nS+1, nM+1];
            int[,] limits = new int[nS+1, nM+1]; ;
            foreach (var line in csvLines)
            {
                deliverycosts[line[0], line[1]] = line[2];
                limits[line[0], line[1]] = line[3];
            }

            //Загрузка в массив прогнозы продаж
            csvLines = new List<int[]>();
            Console.WriteLine("Укажите путь до csv файла с прогнозами. Структура файла по столбцам: id склада (0-4), id магазина (0-9), стоимость, лимит перевозки. Или нажмите Enter для загрузки forecast.csv");
            path = ListenPath("forecast.csv");
            foreach (string line in File.ReadAllLines(path))
            {
                var tmp = Array.ConvertAll(line.Split(','), item => Int32.Parse(item));
                if (tmp[0] > nD) nD = tmp[0];
                if (tmp[1] > nP) nP = tmp[1];
                if (tmp[2] > nM) nM = tmp[2];
                csvLines.Add(tmp);
            }
            int[,,] forecast = new int[nD+1, nP+1, nM+1];
            foreach (var line in csvLines) forecast[line[0], line[1], line[2]] = line[3];

            SolveTransportTask(products, availability, deliverycosts, limits, forecast);
        }

        //Проверка файла на наличие
        private static string ListenPath(string defaultPath)
        {
            string path = Console.ReadLine();
            if (path.Length == 0) path = defaultPath;
            while (!File.Exists(path))
            {
                Console.WriteLine("Файл " + path + " не найден. Введите путь заново.");
                path = Console.ReadLine();
            }
            return path;
        }
    }

}
