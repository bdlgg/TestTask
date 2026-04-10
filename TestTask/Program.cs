using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestTask
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Выберите задание:");
            Console.WriteLine("1 - Компрессия / декомпрессия строки");
            Console.WriteLine("2 - Потокобезопасный сервер");
            Console.WriteLine("3 - Стандартизация лог-файла");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    RunFirst();
                    break;
                case "2":
                    RunSecond();
                    break;
                case "3":
                    RunThird();
                    break;
                default:
                    Console.WriteLine("Нужно выбрать 1 из 3 заданий");
                    break;
            }
        }

        static void RunFirst()
        {
            Console.Write("Введите строку: ");
            string str = Console.ReadLine();
            if (string.IsNullOrEmpty(str))
            {
                Console.WriteLine("Строка пустая");
                return;
            }

            string compressed = Compression.Compress(str);
            string decompressed = Compression.Decompress(compressed);
            Console.WriteLine("Сжатая строка: " + compressed);
            Console.WriteLine("Исходная строка: " + decompressed);
        }

        static void RunSecond()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 1; i <= 3; i++)
            {
                int readerId = i;
                tasks.Add(Task.Run(() =>
                {
                    int value = Server.GetCount();
                    Console.WriteLine($"Читатель {readerId}: count = {value}");
                }));
            }

            for (int i = 1; i <= 3; i++)
            {
                int writerId = i;
                int addValue = i * 10;
                tasks.Add(Task.Run(() =>
                {
                    Console.WriteLine($"Писатель {writerId}: добавил {addValue}");
                    Server.AddToCount(addValue);
                    Console.WriteLine($"Писатель {writerId} закончил");
                }));
            }
            for (int i = 4; i <=6; i++)
            {
                int readerId = i;
                tasks.Add(Task.Run(() =>
                {
                    int value = Server.GetCount();
                    Console.WriteLine($"Читатель: {readerId}: count = {value}");
                }));
            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("Итоговое значение count: " + Server.GetCount());
        }

        static void RunThird()
        {
            Console.WriteLine("Введите путь к входному лог-файлу");
            string inputPath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(inputPath))
            {
                Console.WriteLine("Путь к входному файлу не указан");
                return;
            }
            Console.WriteLine("Введите путь к выходному файлу");
            string outputPath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                Console.WriteLine("Путь к выходному файлу не указан");
                return;
            }
            string baseDir = AppContext.BaseDirectory;
            string projectPath = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;
            if (!Path.IsPathRooted(inputPath))
                inputPath = Path.Combine(projectPath, inputPath);
            if (!Path.IsPathRooted(outputPath))
                outputPath = Path.Combine(projectPath, outputPath);
            string outputDir = Path.GetDirectoryName(outputPath)!;
            string problemsPath = Path.Combine(outputDir, "problems.txt");
            
            try
            {
                LogProcessor.ProcessFile(inputPath, outputPath, problemsPath);
                Console.WriteLine("Нормализованные записи сохранены в: " + outputPath);
                Console.WriteLine("Невалидные записи сохранены в: " + problemsPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при обработке файла: " + ex.Message);
            }
        }
    }
};

