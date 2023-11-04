﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace laba
{
    public class Player
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int TotalMoves { get; set; }
        public int WinsCount { get; set; }
        public int GameProgress { get; set; }

        public Player(string name, string password)
        {
            Name = name;
            Password = password;
            TotalMoves = 0;
            WinsCount = 0;
            GameProgress = 0;
        }

        public float AverageMovesPerWin()
        {
            if (WinsCount == 0) return TotalMoves;
            return (float) TotalMoves / WinsCount;
        }
    }

    class Program
    {
        private static int _sticks = 20;
        private static int _sticksTaken = 0;
        private static int _currentPlayer = 1;
        private static Random _random = new Random();
        private static List<Player> _players = new List<Player>();

        static void Main(string[] args)
        {
            LoadPlayersFromFile("players.json");
            Console.WriteLine("Добро пожаловать в игру Ним!");

            bool gameRunning = true;

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

        static void ShowLeaderboard()
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

        static void PlayGame()
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
            string password = Console.ReadLine();

            if (password != _players[profileIndex].Password)
            {
                Console.WriteLine("Неверный пароль. Игра не может быть начата.");
                return;
            }

            if (_players[profileIndex].GameProgress > 0)
            {
                Console.WriteLine("Обнаружен прогресс игры. Загрузить? (Y/N)");
                string answer = Console.ReadLine().ToLower();
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

            if (!isLoaded) { 
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
                    _sticksTaken = GetValidInput(1, 3, true, profileIndex);
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
        }

        static void SaveTheGame(int? profileIndex)
        {
            _players[profileIndex.Value].GameProgress = 20 - _sticks; // Сохраняем прогресс игры
            JsonHelper.SavePlayersToFile(_players, "players.json");
            Console.WriteLine("\nПрогресс игры сохранен!");
        }

        //static int GetValidInput(int min, int max, bool isGame=false, int? profileIndex=null)
        //{
        //    int input = min - 1;
        //    while (input < min || input > max)
        //    {
        //        if (int.TryParse(Console.ReadLine(), out int tests))
        //        {
        //            input = tests;
        //        }
        //    }

        //    return input;
        //}

        static int GetValidInput(int min, int max, bool isGame = false, int? profileIndex = null)
        {
            int input = min - 1;
            string inputBuffer = ""; // Буфер для сбора введенных цифр

            while (input < min || input > max)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

                // Обработка Ctrl+S для сохранения игры
                if (keyInfo.Modifiers == ConsoleModifiers.Control && keyInfo.Key == ConsoleKey.S && isGame)
                {
                    SaveTheGame(profileIndex);
                }
                // Обработка числового ввода
                else if (char.IsDigit(keyInfo.KeyChar))
                {
                    inputBuffer += keyInfo.KeyChar;
                    Console.Write(keyInfo.KeyChar); // Отображаем введенную цифру
                }
                // Обработка стирания символов
                else if (keyInfo.Key == ConsoleKey.Backspace && inputBuffer.Length > 0)
                {
                    inputBuffer = inputBuffer.Remove(inputBuffer.Length - 1);
                    Console.Write("\b \b"); // Стираем символ с консоли
                }
                // Обработка нажатия Enter
                else if (keyInfo.Key == ConsoleKey.Enter && inputBuffer.Length > 0)
                {
                    if (int.TryParse(inputBuffer, out int tests) && tests >= min && tests <= max)
                    {
                        input = tests;
                        Console.Write(keyInfo.KeyChar);
                    }
                    else
                    {
                        // Console.WriteLine("\nНеверный ввод. Попробуйте еще раз:");
                        inputBuffer = ""; // Очищаем буфер, если введено некорректное значение
                    }
                }
            }

            return input;
        }

        static int GetComputerMove()
        {
            int taken = _random.Next(1, Math.Min(3, _sticks));
            return taken;
        }

        static void CreateProfile()
        {
            Console.Write("Введите имя нового игрока: ");
            string name = Console.ReadLine();

            Console.Write("Введите пароль нового игрока: ");
            string password = Console.ReadLine();

            var player = new Player(name, password);
            _players.Add(player);
        }

        private static void LoadPlayersFromFile(string filename)
        {
            if (File.Exists(filename))
            {
                string json = File.ReadAllText(filename);
                _players = JsonConvert.DeserializeObject<List<Player>>(json);
            }
        }

        public static class JsonHelper
        {
            public static void SavePlayersToFile(List<Player> players, string filename)
            {
                string json = JsonConvert.SerializeObject(players);
                File.WriteAllText(filename, json);
            }
        }
    }
}

