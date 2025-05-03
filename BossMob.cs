using Godot;

public partial class BossMob : Mob
{
    private const int InitialLifeCycles = 15;
    
    public static int Speed { get; } = 100;
    
    private int _lifeCycles = InitialLifeCycles;

    protected override string SelectAnimation(AnimatedSprite2D sprite)
    {
        return sprite.Animation;
    }

    public void Recalibrate(Vector2 playerPosition)
    {
        var velocity = new Vector2(Speed, 0);
        var newAngle = (playerPosition - Position).Angle();
        
        Rotation = newAngle;
        LinearVelocity = velocity.Rotated(newAngle);

        _lifeCycles--;
        if (_lifeCycles == 0)
        {
            // TODO: puff animation
            QueueFree();
        }
    }
}
