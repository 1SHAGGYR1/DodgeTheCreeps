using Godot;
using System;

public partial class Ammo : Area2D
{
    [Signal]
    public delegate void DisappearEventHandler();

    private void OnLifeTimerTimeout()
    {
        EmitSignal(SignalName.Disappear);
        QueueFree();
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is Player)
        {
            GetNode<AudioStreamPlayer>("AmmoPickUpSound").Play();
            Visible = false;
            GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
            EmitSignal(SignalName.Disappear);
        }
    }

    private void OnAmmoPickUpSoundFinished()
    {
        QueueFree();
    }
}
