using Newtonsoft.Json;// Использование библиотеки Newtonsoft для работы с JSON-форматом


namespace laba
{
    // Определение класса Player
    public class Player
    {
        // Свойства объектов Player
        public string Name { get; set; }
        public string Password { get; set; }
        public int TotalMoves { get; set; }
        public int WinsCount { get; set; }
        public int GameProgress { get; set; }
        
        // Конструктор Player с проверкой на Null значение
        public Player(string name, string password)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            TotalMoves = 0;
            WinsCount = 0;
            GameProgress = 0;
        }
        
        // Метод для подсчета среднего кол-ва ходов на победу
        public float AverageMovesPerWin()
        {
            if (WinsCount == 0) return TotalMoves;
            return (float) TotalMoves / WinsCount;
        }
    }
    
    // Класс Program с логикой игры
    class Program
    {
        // Инициализация и объявление переменных
        private static int _sticks;
        private static int _sticksTaken;
        private static int _currentPlayer;
        private static readonly Random Random = new Random();
        private static List<Player> _players = new List<Player>();

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
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(intercept: true);

                // Обработка Ctrl + S для сохранения игры
                if (keyInfo.Modifiers == ConsoleModifiers.Control && keyInfo.Key == ConsoleKey.S && isGame)
                {
                    SaveTheGame(profileIndex);
                    return -1;
                }

            } while (!int.TryParse(keyInfo.KeyChar.ToString(), out int input) || input < min || input > max);

            return int.Parse(keyInfo.KeyChar.ToString());
        }

        static int GetComputerMove()
        {
            int taken = Random.Next(1, Math.Min(3, _sticks));
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

