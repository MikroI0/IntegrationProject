import json

# Функция для вычисления статистики
def calculate_statistics(numbers):
    """
    Принимает список чисел, возвращает словарь с результатами
    """
    if not numbers:
        return {
            "error": "Список чисел пуст",
            "sum": None,
            "average": None,
            "min": None,
            "max": None
        }
    
    return {
        "error": None,
        "sum": sum(numbers),                    
        "average": sum(numbers) / len(numbers), 
        "min": min(numbers),                    
        "max": max(numbers)                     
    }

def main():
    input_file = "input.json"
    output_file = "output.json"
    
    try:
        with open(input_file, "r", encoding="utf-8") as f:
            data = json.load(f)
            numbers = data.get("numbers", [])
        result = calculate_statistics(numbers)
        
        with open(output_file, "w", encoding="utf-8") as f:
            json.dump(result, f, ensure_ascii=False, indent=2)
            
    except Exception as e:
        error_result = {"error": f"Ошибка: {str(e)}"}
        with open(output_file, "w", encoding="utf-8") as f:
            json.dump(error_result, f)

if __name__ == "__main__":
    main()