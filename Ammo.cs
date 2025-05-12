using Godot;
using System;

public partial class Ammo : Area2D
{

    private void OnLifeTimerTimeout()
    {
        QueueFree();
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is Player)
        {
            QueueFree();
        }
    }
}
