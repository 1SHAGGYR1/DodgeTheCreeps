using Godot;

public partial class TrackingMob : Area2D
{
    [Export] public int Speed { get; set; } = 100;
    
    [Signal]
    public delegate void DeathEventHandler();
    
    public override void _Ready()
    {
        var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        sprite.Play(sprite.Animation);
    }

    public override void _Process(double delta)
    {
        var player = (GetTree().GetFirstNodeInGroup("Player") as Player)!;
        base._Process(delta);
        
        var velocity = new Vector2(Speed, 0);
        var newAngle = (player.Position - Position).Angle();

        Position += velocity.Rotated(newAngle) * (float) delta;
        Rotation = newAngle;
    }

    private void OnDeathTimerTimeout()
    {
        EmitSignal(SignalName.Death);
        // TODO: puff animation
        QueueFree();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is PlayerProjectile projectile)
        {
            EmitSignal(SignalName.Death);
            projectile.Remove(); 
            QueueFree();
        }
    }
}
