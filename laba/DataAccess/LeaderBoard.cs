using laba.BusinessLogic;
using laba.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laba.DataAccess
{
    internal class LeaderBoard
    {
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

    }
}
