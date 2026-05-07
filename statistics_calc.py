import json

# Функция для вычисления статистики
def calculate_statistics(numbers):
    """
    Принимает список чисел, возвращает словарь с результатами
    """
    # Если список пустой - возвращаем ошибку
    if not numbers:
        return {
            "error": "Список чисел пуст",
            "sum": None,
            "average": None,
            "min": None,
            "max": None
        }
    
    # Вычисляем всё, что нужно
    return {
        "error": None,
        "sum": sum(numbers),                    # сумма
        "average": sum(numbers) / len(numbers), # среднее
        "min": min(numbers),                    # минимум
        "max": max(numbers)                     # максимум
    }

# Главная функция - читает input.json, пишет output.json
def main():
    input_file = "input.json"
    output_file = "output.json"
    
    try:
        # Читаем числа из файла
        with open(input_file, "r", encoding="utf-8") as f:
            data = json.load(f)
            numbers = data.get("numbers", [])
        
        # Вычисляем статистику
        result = calculate_statistics(numbers)
        
        # Сохраняем результат в файл
        with open(output_file, "w", encoding="utf-8") as f:
            json.dump(result, f, ensure_ascii=False, indent=2)
            
    except Exception as e:
        # Если что-то пошло не так - сохраняем ошибку
        error_result = {"error": f"Ошибка: {str(e)}"}
        with open(output_file, "w", encoding="utf-8") as f:
            json.dump(error_result, f)

# Точка входа в программу
if __name__ == "__main__":
    main()