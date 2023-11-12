using Newtonsoft.Json;
// Использование библиотеки Newtonsoft для работы с JSON-форматом

namespace laba
{
    public class PlayerManager
    {
        private List<Player> _players;
        private const string PlayersFileName = "players.json";

        public PlayerManager()
        {
            _players = LoadPlayersFromFile();
        }

        public List<Player> GetPlayers()
        {
            return _players;
        }
        private List<Player> LoadPlayersFromFile()
        {
            if (File.Exists(PlayersFileName))
            {
                string json = File.ReadAllText(PlayersFileName);
                return JsonConvert.DeserializeObject<List<Player>>(json) ?? new List<Player>();
            }
            return new List<Player>();
        }

        public void SavePlayersToFile()
        {
            string json = JsonConvert.SerializeObject(_players);
            File.WriteAllText(PlayersFileName, json);
        }

        public void CreateProfile()
        {
            Console.Write("Введите имя нового игрока: ");
            string name = Console.ReadLine() ?? string.Empty;

            if (_players.Any(p => p.Name == name))
            {
                Console.WriteLine("Игрок с таким именем уже существует. Попробуйте другое имя.");
                return;
            }

            Console.Write("Введите пароль нового игрока: ");
            string password = Console.ReadLine() ?? string.Empty;

            Player player = new Player(name, password);
            _players.Add(player);
            Console.WriteLine("Новый профиль создан.");
            SavePlayersToFile();
        }

        public void DeletePlayer()
        {
            if (_players.Count == 0)
            {
                Console.WriteLine("Нет доступных профилей для удаления.");
                return;
            }

            Console.WriteLine("Выберите профиль для удаления:");
            for (int i = 0; i < _players.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_players[i].Name}");
            }

            int profileIndex = GetValidInput(1, _players.Count) - 1;

            _players.RemoveAt(profileIndex);
            Console.WriteLine("Профиль успешно удален.");
            SavePlayersToFile();
        }

        private int GetValidInput(int min, int max)
        {
            int input;
            do
            {
                Console.Write("Введите число: ");
            } while (!int.TryParse(Console.ReadLine(), out input) || input < min || input > max);

            return input;
        }
    }

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
        private const int InitialSticks = 20;
        private const int MinComputerMove = 1;
        private const int MaxSticksPerTurn = 3;
        private const int WinningSticksCount = 0;

        private static int _sticks;
        private static int _sticksTaken;
        private static int _currentPlayer;
        private static readonly Random Random = new();

        private static PlayerManager _playerManager = new PlayerManager();

        static void Main()
        {
            Console.WriteLine("Добро пожаловать в игру Ним!");

            bool gameRunning = true;
            while (gameRunning)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Начать игру");
                Console.WriteLine("2. Новый игрок");
                Console.WriteLine("3. Список лидеров");
                Console.WriteLine("4. Удалить игрока");
                Console.WriteLine("5. Выход из игры");

                int choice = GetValidInput(1, 5);

                switch (choice)
                {
                    case 1:
                        PlayGame();
                        break;
                    case 2:
                        _playerManager.CreateProfile();
                        break;
                    case 3:
                        ShowLeaderboard();
                        break;
                    case 4:
                        _playerManager.DeletePlayer();
                        break;
                    case 5:
                        Console.WriteLine("Выход из игры.");
                        gameRunning = false;
                        break;
                }
            }

            Console.ReadLine();
        }

        static void ShowLeaderboard()
        {
            // Доступ к _players теперь через _playerManager
            var players = _playerManager.GetPlayers(); // Предполагаем, что такой метод есть в PlayerManager
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

        static void PlayGame() //начать игру
        {
            bool isLoaded = false;

            var players = _playerManager.GetPlayers();
    
            if (players.Count == 0)
            {
                Console.WriteLine("Нет доступных профилей. Создайте новый профиль.");
                return;
            }

            Console.WriteLine("\nВыберите профиль:");

            for (int i = 0; i < _playerManager.GetPlayers().Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_playerManager.GetPlayers()[i].Name}");
            }

            int profileIndex = GetValidInput(1, _playerManager.GetPlayers().Count) - 1;

            Console.Write("Введите пароль: ");
            string? password = Console.ReadLine();

            if (password != _playerManager.GetPlayers()[profileIndex].Password)
            {
                Console.WriteLine("Неверный пароль. Игра не может быть начата.");
                return;
            }

            if (_playerManager.GetPlayers()[profileIndex].GameProgress > 0)
            {
                Console.WriteLine("Обнаружен прогресс игры. Загрузить? (Y/N)");

                string? answer = Console.ReadLine()?.ToLower();

            if (answer == "y")
                {
                    _sticks = InitialSticks - _playerManager.GetPlayers()[profileIndex].GameProgress;
                    _currentPlayer = _playerManager.GetPlayers()[profileIndex].GameProgress % 2 + 1;
                    Console.WriteLine("Прогресс игры успешно загружен.");
                    isLoaded = true;
                }
                else
                {
                    _playerManager.GetPlayers()[profileIndex].GameProgress = 0;
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

                if (_sticks == WinningSticksCount)
                {
                    Console.WriteLine("Игрок " + _currentPlayer + " проиграл!");
                    Console.WriteLine("Количество ходов игрока " + _currentPlayer + ": " + playerMoves);

                    if (!(profileIndex >= _playerManager.GetPlayers().Count))
                    {
                        _playerManager.GetPlayers()[profileIndex].TotalMoves += playerMoves;
                        _playerManager.GetPlayers()[profileIndex].GameProgress = 0;
                    }

                    if (_currentPlayer == 1) _playerManager.GetPlayers()[profileIndex].WinsCount++;

                    return;
                }

                _currentPlayer = _currentPlayer == 1 ? 2 : 1; // меняем игрока
            }
        }
        
        static void SaveTheGame(int? profileIndex, List<Player> players)
        {
            if (profileIndex.HasValue && profileIndex.Value >= 0 && profileIndex.Value < players.Count)
            {
                players[profileIndex.Value].GameProgress = InitialSticks - _sticks;
                _playerManager.SavePlayersToFile();
                Console.WriteLine("\nПрогресс игры сохранен!");
            }
            else
            {
                Console.WriteLine("\nНеверный индекс профиля!");
            }
        }


        static int GetValidInput(int min, int max, bool isGame = false, int? profileIndex = null) // проверка сохраненной игры по указонному индексу
        {
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(intercept: true);

                // Обработка Ctrl + S для сохранения игры
                if (keyInfo.Modifiers == ConsoleModifiers.Control && keyInfo.Key == ConsoleKey.S && isGame)
                {
                    SaveTheGame(profileIndex, _playerManager.GetPlayers());
                    return -1;
                }

            } while (!int.TryParse(keyInfo.KeyChar.ToString(), out int input) || input < min || input > max);

            return int.Parse(keyInfo.KeyChar.ToString());
        }

        static int GetComputerMove() //создаем ограничение компьютера по выбору палочек
        {
            int taken = Random.Next(MinComputerMove, Math.Min(MaxSticksPerTurn, _sticks));
            return taken;
        }
        
        public static class JsonHelper //сохраняем список игроков 
        {
            public static void SavePlayersToFile(List<Player> players, string filename)
            {
                string json = JsonConvert.SerializeObject(players);
                File.WriteAllText(filename, json);
            }
        }
    }
}