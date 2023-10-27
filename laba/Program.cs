using System;
using System.Collections.Generic;
using System.IO;
class Player
{
    public string Login { get; set; }
    public string Password { get; set; }
    
    public Player(string Login, string Password)
    {
        Login = Login;
        Password = Password;
    }
}

class Program
{
    private static int sticks = 20;
    private static int sticksTaken = 0;
    private static int currentPlayer = 1;
    private static Random random = new Random();
    private static List<Player> Players = new List<Player>();

    static void Main(string[] args)
    {
        Console.WriteLine("Добро пожаловать в игру Ним!");

        bool gameRunning = true;

        while (gameRunning)
        {
            Console.WriteLine("\nВыберите действие: ");
            Console.WriteLine("1.Начать игру");
            Console.WriteLine("2.Новый игрок");
            Console.WriteLine("3.Выйти из игры");
            
        }
        
    }

    static void CreateProfile()
    {
        Console.WriteLine("Введите имя нового игрока: ");
        string login = Console.ReadLine();
        
        Console.WriteLine("Введите пароль для нового игрока: ");
        string password = Console.ReadLine();

        Players.Add(new Player(login, password));
        Console.WriteLine("Профиль игрока успешно создан!");
    }
}
