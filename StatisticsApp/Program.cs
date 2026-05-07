using System.Diagnostics;
using Newtonsoft.Json;

namespace StatisticsApp
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Калькулятор статистики (C# + Python) ===\n");

            while (true)
            {
                // 1. Запрашиваем у пользователя числа
                Console.Write("Введите числа через пробел (или 'exit' для выхода): ");
                string userInput = Console.ReadLine();

                // Выход из программы
                if (userInput == "exit")
                    break;

                // 2. Преобразуем строку в массив чисел
                double[] numbers = ParseNumbers(userInput);

                // Проверка: есть ли хоть одно число?
                if (numbers.Length == 0)
                {
                    Console.WriteLine("Ошибка: не введено ни одного числа!\n");
                    continue;
                }

                // 3. Передаём числа в Python и получаем результат
                CallPythonAndShowResult(numbers);

                Console.WriteLine(); // пустая строка для красоты
            }
        }

        /// <summary>
        /// Преобразует строку "10 20 30.5" в массив чисел [10, 20, 30.5]
        /// </summary>
        static double[] ParseNumbers(string input)
        {
            // Если строка пустая - возвращаем пустой массив
            if (string.IsNullOrWhiteSpace(input))
                return new double[0];

            // Разбиваем строку по пробелам
            string[] parts = input.Split(' ');

            // Список для хранения чисел
            List<double> numbers = new List<double>();

            // Пробуем превратить каждую часть в число
            foreach (string part in parts)
            {
                if (double.TryParse(part, out double number))
                {
                    numbers.Add(number);  // получилось - добавляем
                }
                else
                {
                    // Не число - выводим предупреждение
                    Console.WriteLine($"Предупреждение: '{part}' не является числом, пропускаем");
                }
            }

            return numbers.ToArray();
        }

        /// <summary>
        /// Запускает Python-скрипт и выводит результат
        /// </summary>
        static void CallPythonAndShowResult(double[] numbers)
        {
            // Имена файлов для обмена данными
            string inputFile = "input.json";
            string outputFile = "output.json";
            string pythonScript = "../statistics_calc.py";

            // Шаг 1: сохраняем числа в JSON-файл
            var data = new { numbers = numbers };
            string jsonInput = JsonConvert.SerializeObject(data);
            File.WriteAllText(inputFile, jsonInput);
            Console.WriteLine($"Отправляем в Python: {jsonInput}");

            // Шаг 2: запускаем Python-скрипт
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "python";          // команда для запуска Python
                process.StartInfo.Arguments = pythonScript;     // имя скрипта
                process.StartInfo.UseShellExecute = false;      // не использовать оболочку Windows
                process.StartInfo.CreateNoWindow = true;        // не показывать окно Python
                process.StartInfo.RedirectStandardError = true; // перехватываем ошибки

                process.Start();    // запускаем Python
                process.WaitForExit(); // ждём завершения

                // Если Python выдал ошибку - показываем
                string errors = process.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(errors))
                    Console.WriteLine($"Ошибка Python: {errors}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось запустить Python: {ex.Message}");
                Console.WriteLine("Убедитесь, что Python установлен и доступен в командной строке");
                return;
            }

            // Шаг 3: читаем результат от Python
            if (!File.Exists(outputFile))
            {
                Console.WriteLine("Ошибка: Python не создал файл с результатами");
                return;
            }

            string jsonOutput = File.ReadAllText(outputFile);
            Console.WriteLine($"Получено от Python: {jsonOutput}");

            // Шаг 4: превращаем JSON в объект и выводим результат
            dynamic result = JsonConvert.DeserializeObject(jsonOutput);

            if (result.error != null)
            {
                Console.WriteLine($"Ошибка: {result.error}");
            }
            else
            {
                Console.WriteLine("\n========== РЕЗУЛЬТАТЫ ==========");
                Console.WriteLine($"Сумма чисел:     {result.sum}");
                Console.WriteLine($"Среднее значение: {result.average:F2}");
                Console.WriteLine($"Минимальное число: {result.min}");
                Console.WriteLine($"Максимальное число: {result.max}");
                Console.WriteLine("==================================");
            }
        }
    }
}