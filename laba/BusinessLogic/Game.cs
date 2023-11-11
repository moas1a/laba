using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace laba.BusinessLogic;
class Game
{
    // Инициализация и объявление переменных
    private static int _sticks;
    private static int _sticksTaken;
    private static int _currentPlayer;
    private static readonly Random Random = new();
    private static List<Player> _players = new();

    // Основной метод входа в программу
    static void Main()
    {
        // Загрузка игроков из файла
        LoadPlayersFromFile("players.json");
        Console.WriteLine("Добро пожаловать в игру Ним!");

        // Запускаем игру и отображаем главное меню
        bool gameRunning = true;

        // Главный игровой цикл
        while (gameRunning)
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1. Начать игру");
            Console.WriteLine("2. Новый игрок");
            Console.WriteLine("3. Список Лидеров");
            Console.WriteLine("4. Выход из игры");

            int choice = GetValidInput(1, 4);

            switch (choice)
            {
                case 1:
                    PlayGame();
                    JsonHelper.SavePlayersToFile(_players, "players.json");
                    break;
                case 2:
                    CreateProfile();
                    JsonHelper.SavePlayersToFile(_players, "players.json");
                    break;
                case 3:
                    ShowLeaderboard();
                    break;
                case 4:
                    Console.WriteLine("Выход из игры.");
                    gameRunning = false;
                    break;
            }
        }

        Console.ReadLine();
    }
    /*
            static void ShowLeaderboard() //показать таблицу лидеров
            {
                if (_players.Count == 0)
                {
                    Console.WriteLine("Нет доступных профилей.");
                    return;
                }

                var sortedPlayers = _players.OrderBy(p => p.AverageMovesPerWin()).ToList();
                Console.WriteLine("\nТаблица лидеров:");
                for (var i = 0; i < sortedPlayers.Count; i++)
                {
                    Console.WriteLine(
                        $"{i + 1}. {sortedPlayers[i].Name} - среднее число ходов за игру: {sortedPlayers[i].AverageMovesPerWin()}");
                }
            }
    */
    static void PlayGame() //начать игру
    {
        bool isLoaded = false;

        if (_players.Count == 0)
        {
            Console.WriteLine("Нет доступных профилей. Создайте новый профиль.");
            return;
        }

        Console.WriteLine("\nВыберите профиль:");

        for (int i = 0; i < _players.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_players[i].Name}");
        }

        int profileIndex = GetValidInput(1, _players.Count) - 1;

        Console.Write("Введите пароль: ");
        string? password = Console.ReadLine();

        if (password != _players[profileIndex].Password)
        {
            Console.WriteLine("Неверный пароль. Игра не может быть начата.");
            return;
        }

        if (_players[profileIndex].GameProgress > 0)
        {
            Console.WriteLine("Обнаружен прогресс игры. Загрузить? (Y/N)");

            string? answer = Console.ReadLine()?.ToLower();

            if (answer == "y")
            {
                _sticks = 20 - _players[profileIndex].GameProgress;
                _currentPlayer = _players[profileIndex].GameProgress % 2 + 1;
                Console.WriteLine("Прогресс игры успешно загружен.");
                isLoaded = true;
            }
            else
            {
                _players[profileIndex].GameProgress = 0;
                Console.WriteLine("Прогресс игры сброшен.");
            }

        }

        if (!isLoaded)
        {
            _sticks = 20;
            _currentPlayer = 1;
        }

        Console.WriteLine("\nИгра началась!");

        int playerMoves = 0;

        while (_sticks > 0)
        {
            Console.WriteLine("\nНа столе осталось " + _sticks + " палочек.");

            if (_currentPlayer == 1)
            {
                Console.Write("Ваш ход. Сколько палочек вы хотите взять (1-3)? ");
                var inputForValidation = GetValidInput(1, 3, true, profileIndex);
                if (inputForValidation == -1) return;
                _sticksTaken = inputForValidation;
            }
            else
            {
                _sticksTaken = GetComputerMove();
                Console.WriteLine("Ход компьютера: " + _sticksTaken);
            }

            if (_currentPlayer == 1)
            {
                playerMoves++; // увеличиваем количество ходов игрока
            }

            _sticks -= _sticksTaken;

            if (_sticks == 0)
            {
                Console.WriteLine("Игрок " + _currentPlayer + " проиграл!");
                Console.WriteLine("Количество ходов игрока " + _currentPlayer + ": " + playerMoves);

                if (!(profileIndex >= _players.Count))
                {
                    _players[profileIndex].TotalMoves += playerMoves;
                    _players[profileIndex].GameProgress = 0;
                }

                if (_currentPlayer == 1) _players[profileIndex].WinsCount++;

                return;
            }

            _currentPlayer = _currentPlayer == 1 ? 2 : 1; // меняем игрока
        }
        static void SaveTheGame(int? profileIndex, List<Player> players) //сохраняем прогресс игры для определенного профиля
        {
            if (profileIndex.HasValue) // Проверяем, что "profileIndex" имеет значение. Если это так, то используем его для получения конкретного игрового профиля из списка "players".
            {
                int index = profileIndex.Value;

                if (index >= 0 && index < players.Count) // Проверяем, что индекс находится в допустимом диапазоне.
                {
                    players[index].GameProgress = 20 - _sticks;
                    JsonHelper.SavePlayersToFile(players, "players.json");
                    Console.WriteLine("\nПрогресс игры сохранен!");
                }
                else
                {
                    Console.WriteLine("\nНеверный индекс профиля!");
                }
            }
        }
    }
}