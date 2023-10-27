using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

public static class JsonHelper
{
    public static void SavePlayersToFile(List<Player> players, string filePath)
    {
        try
        {
            var json = JsonConvert.SerializeObject(players, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при сохранении данных: " + ex.Message);
        }
    }
    public static List<Player> LoadPlayersFromFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath)) return new List<Player>();
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Player>>(json) ?? new List<Player>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при загрузке данных: " + ex.Message);
            return new List<Player>();
        }
    }
    public static void SaveDictionaryToJsonFile(Dictionary<string, string> dictionary, string filePath)
    {
        try
        {
            // Конвертируем словарь в строку JSON
            string json = JsonConvert.SerializeObject(dictionary, Formatting.Indented);

            // Сохраняем JSON строку в файл
            File.WriteAllText(filePath, json);

            Console.WriteLine("Данные успешно сохранены в файл: " + filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Возникла ошибка при сохранении данных: " + ex.Message);
        }
    }
}
public class Player {
    public string Name { get; set; }
    public string Password { get; set; }
    public int TotalMoves { get; set; }
    public int WinsCount { get; set; }
    public Player(string name, string password) {
        Name = name;
        Password = password;
        TotalMoves = 0;
        WinsCount = 0;
    }
    public float AverageMovesPerWin()
    {
        if (WinsCount == 0) return TotalMoves;
        return (float) TotalMoves / WinsCount;
    }
}

class Program {
    static int sticks = 20;
    static int sticksTaken = 0;
    static int currentPlayer = 1;
    static Random random = new Random();
    static List<Player> players = new List<Player>();

    static void Main(string[] args) {
        var players = JsonHelper.LoadPlayersFromFile("players.json");  // загружаем данные
        Console.WriteLine("Добро пожаловать в игру Ним!");

        bool gameRunning = true;

        while (gameRunning) {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1. Начать игру");
            Console.WriteLine("2. Новый игрок");
            Console.WriteLine("3. Список Лидеров:)");
            Console.WriteLine("4. Выход из игры!");

            int choice = GetValidInput(1, 4);

            switch (choice) {
                case 1:
                    PlayGame();
                    JsonHelper.SavePlayersToFile(players, "players.json");  // сохраняем данные
                    break;
                case 2:
                    CreateProfile();
                    JsonHelper.SavePlayersToFile(players, "players.json");  // сохраняем данные
                    break;
                case 3:
                    ShowLeaderboard();
                    break;
                case 4:
                    Console.WriteLine("Выход из игры.");
                    Environment.Exit(1);
                    break;
            }
        }

        Console.ReadLine();
    }
    
    static void ShowLeaderboard()
    {
        if (players.Count == 0)
        {
            Console.WriteLine("Нет доступных профилей.");
            return;
        }
        var sortedPlayers = players.OrderBy(p => p.AverageMovesPerWin()).ToList();
        Console.WriteLine("\nТаблица лидеров:");
        for (var i = 0; i < sortedPlayers.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {sortedPlayers[i].Name} - среднее число ходов за игру: {sortedPlayers[i].AverageMovesPerWin()}");
        }
    }
    
    static void PlayGame() {
        if (players.Count == 0) {
            Console.WriteLine("Нет доступных профилей. Создайте новый профиль.");
            return;
        }

        Console.WriteLine("\nВыберите профиль:");

        for (int i = 0; i < players.Count; i++) {
            Console.WriteLine($"{i + 1}. {players[i].Name}");
        }

        int profileIndex = GetValidInput(1, players.Count) - 1;

        Console.Write("Введите пароль: ");
        string password = Console.ReadLine();

        if (password != players[profileIndex].Password) {
            Console.WriteLine("Неверный пароль. Игра не может быть начата.");
            return;
        }

        sticks = 20;
        currentPlayer = 1;

        Console.WriteLine("\nИгра началась!");
        
        int playerMoves = 0;
        
        while (sticks > 0) {
            Console.WriteLine("\nНа столе осталось " + sticks + " палочек.");

            if (currentPlayer == 1) {
                Console.Write("Ваш ход. Сколько палочек вы хотите взять (1-3)? ");
                sticksTaken = GetValidInput(1, 3);
            } else {
                sticksTaken = GetComputerMove();
                Console.WriteLine("Ход компьютера: " + sticksTaken);
            }
            if (currentPlayer == 1) 
            {
                playerMoves++;  // увеличиваем количество ходов игрока
            }

            sticks -= sticksTaken;

            if (sticks <= 0) {
                if (currentPlayer == 1) {
                    Console.WriteLine("Поздравляем! Вы победили!");
                    players[profileIndex].WinsCount++;  // увеличиваем количество побед игрока
                    players[profileIndex].TotalMoves += playerMoves;  // добавляем количество ходов
                } else {
                    Console.WriteLine("К сожалению, компьютер победил.");
                }
                JsonHelper.SavePlayersToFile(players, "players.json");  // сохраняем данные
                break;
            }

            currentPlayer = currentPlayer == 1 ? 2 : 1;
        }
    }

    static void CreateProfile() {
        Console.Write("Введите имя нового игрока: ");
        string name = Console.ReadLine();

        Console.Write("Введите пароль для нового игрока: ");
        string password = Console.ReadLine();

        players.Add(new Player(name, password));
        Console.WriteLine("Профиль игрока успешно создан!");
    }

    static int GetValidInput(int min, int max) {
        int input = 0;
        bool isValid = false;

        while (!isValid) {
            try {
                input = int.Parse(Console.ReadLine());

                if (input >= min && input <= max) {
                    isValid = true;
                } else {
                    Console.WriteLine($"Введите число от {min} до {max}.");
                }
            } catch {
                Console.WriteLine("Некорректный ввод. Повторите попытку.");
            }
        }

        return input;
    }

    static int GetComputerMove() {
        if (sticks == 1) {
            return 1;
        }

        return random.Next(1, 4);
    }
}
