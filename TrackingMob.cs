using Godot;

public partial class TrackingMob : Area2D
{
    [Export] public int Speed { get; set; } = 100;
    
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

    private void OnVisibleOnScreenNotifier2dScreenExited()
    {
        QueueFree();
    }

    private void OnDeathTimerTimeout()
    {
        // TODO: puff animation
        QueueFree();
    }
}
