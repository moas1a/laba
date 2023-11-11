using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laba.BusinessLogic;
  
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
                return (float)TotalMoves / WinsCount;
            }
        }
    

