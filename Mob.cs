using Godot;
using System;

public partial class Mob : RigidBody2D
{
    public override void _Ready()
    {
        var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        var animation = SelectAnimation(sprite);
        sprite.Play(animation);
    }

    protected virtual string SelectAnimation(AnimatedSprite2D sprite)
    {
        var mobTypeAnimations = sprite.SpriteFrames.GetAnimationNames();
        return mobTypeAnimations[Random.Shared.Next(mobTypeAnimations.Length)];
    }

    private void OnVisibleOnScreenNotifier2dScreenExited()
    {
        QueueFree();
    }
}