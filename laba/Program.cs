using System;
using System.Collections.Generic;

class Player
{
    public string Name { get; set; }
    public string Password { get; set; }
    
    public Player(string Name, string Password)
    {
        Name = Name;
        Password = Password;
    }
}
