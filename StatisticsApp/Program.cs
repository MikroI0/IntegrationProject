using System.Diagnostics;
using System.Globalization;
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
                Console.Write("Введите числа через пробел (или 'exit' для выхода): ");
                string? userInput = Console.ReadLine();

                if (userInput == "exit")
                    break;

                double[] numbers = ParseNumbers(userInput ?? "");

                if (numbers.Length == 0)
                {
                    Console.WriteLine("Ошибка: не введено ни одного числа!\n");
                    continue;
                }

                CallPythonAndShowResult(numbers);
                Console.WriteLine();
            }
        }

        static double[] ParseNumbers(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Array.Empty<double>();

            string[] parts = input.Split(' ');
            List<double> numbers = new List<double>();

            foreach (string part in parts)
            {
                if (double.TryParse(part, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                {
                    numbers.Add(number);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(part))
                        Console.WriteLine($"Предупреждение: '{part}' не является числом, пропускаем");
                }
            }

            return numbers.ToArray();
        }

        static void CallPythonAndShowResult(double[] numbers)
        {
            string inputFile = "input.json";
            string outputFile = "output.json";
            string pythonScript = "../statistics_calc.py";

            if (File.Exists(outputFile))
                File.Delete(outputFile);

            var data = new { numbers = numbers };
            string jsonInput = JsonConvert.SerializeObject(data);
            File.WriteAllText(inputFile, jsonInput);
            Console.WriteLine($"Отправляем в Python: {jsonInput}");

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "python";
                process.StartInfo.Arguments = pythonScript;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;

                process.Start();
                process.WaitForExit();

                string errors = process.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(errors))
                {
                    Console.WriteLine($"Ошибка Python: {errors}");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось запустить Python: {ex.Message}");
                return;
            }

            if (!File.Exists(outputFile))
            {
                Console.WriteLine("Ошибка: Python не создал файл с результатами");
                return;
            }

            string jsonOutput = File.ReadAllText(outputFile);
            Console.WriteLine($"Получено от Python: {jsonOutput}");

            var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonOutput);
            
            if (result != null && result.ContainsKey("error") && result["error"] != null && result["error"].ToString() != "")
            {
                Console.WriteLine($"Ошибка: {result["error"]}");
            }
            else if (result != null)
            {
                Console.WriteLine("\n========== РЕЗУЛЬТАТЫ ==========");
                Console.WriteLine($"Сумма чисел:     {result["sum"]}");
                Console.WriteLine($"Среднее значение: {Convert.ToDouble(result["average"]):F2}");
                Console.WriteLine($"Минимальное число: {result["min"]}");
                Console.WriteLine($"Максимальное число: {result["max"]}");
                Console.WriteLine("==================================");
            }
        }
    }
}