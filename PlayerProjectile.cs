using Godot;
using System;

public partial class PlayerProjectile : RigidBody2D
{
    [Export] public int Speed { get; set; } = 600;

    private void OnVisibleOnScreenEnabler2dScreenExited()
    {
        QueueFree();
    }
    
    public void Remove()
    {
        QueueFree();
    }
}
