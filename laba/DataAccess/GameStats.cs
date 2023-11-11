using laba.BusinessLogic;
using laba.Presentation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace laba.DataAccess
{
    internal class GameStats
    {
        static int GetValidInput(int min, int max, bool isGame = false, int? profileIndex = null) // проверка сохраненной игры по указонному индексу
        {
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(intercept: true);

                // Обработка Ctrl + S для сохранения игры
                if (keyInfo.Modifiers == ConsoleModifiers.Control && keyInfo.Key == ConsoleKey.S && isGame)
                {
                    SaveTheGame(profileIndex, _players);
                    return -1;
                }

            } while (!int.TryParse(keyInfo.KeyChar.ToString(), out int input) || input < min || input > max);

            return int.Parse(keyInfo.KeyChar.ToString());
        }

     

        static void CreateProfile() // создаем профиль
        {
            Console.Write("Введите имя нового игрока: ");
            string name = Console.ReadLine() ?? string.Empty;

            Console.Write("Введите пароль нового игрока: ");
            string password = Console.ReadLine() ?? string.Empty;

            Player player = new(name, password);
            _players.Add(player);
        }

        static int GetComputerMove() //создаем ограничение компьютера по выбору палочек
        {
            int taken = Random.Next(1, Math.Min(3, _sticks));
            return taken;
        }
        private static void LoadPlayersFromFile(string filename) //загрузка данных об игроках из файла в программу
        {
            if (File.Exists(filename))
            {
                string json = File.ReadAllText(filename);
                _players = JsonConvert.DeserializeObject<List<Player>>(json) ?? new List<Player>();
            }
            else
            {
                _players = new List<Player>();
            }
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
