using System;
using System.Collections.Generic;

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
