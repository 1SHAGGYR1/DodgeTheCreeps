using Godot;

public partial class Player : Area2D
{
    [Signal]
    public delegate void HitEventHandler();

    [Export] public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).

    public Vector2 ScreenSize; // Size of the game window.

    private const string MoveUpAction = "move_up";
    private const string MoveDownAction = "move_down";
    private const string MoveLeftAction = "move_left";
    private const string MoveRightAction = "move_right";

    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        Hide();
    }

    public override void _Process(double delta)
    {
        var velocity = Vector2.Zero;

        if (Input.IsActionPressed(MoveUpAction))
        {
            velocity.Y -= Speed;
        }

        if (Input.IsActionPressed(MoveDownAction))
        {
            velocity.Y += Speed;
        }

        if (Input.IsActionPressed(MoveLeftAction))
        {
            velocity.X -= Speed;
        }

        if (Input.IsActionPressed(MoveRightAction))
        {
            velocity.X += Speed;
        }

        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            if (velocity.X != 0)
            {
                animatedSprite2D.Animation = "walk";
                animatedSprite2D.FlipV = false;
                // See the note below about the following boolean assignment.
                animatedSprite2D.FlipH = velocity.X < 0;
            }
            else if (velocity.Y != 0)
            {
                animatedSprite2D.Animation = "up";
                animatedSprite2D.FlipV = velocity.Y > 0;
            }

            animatedSprite2D.Play();

            Position += velocity * (float)delta;
            Position = new Vector2(
                x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
                y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
            );
        }
        else
        {
            animatedSprite2D.Stop();
        }
    }
    
    public void Start(Vector2 position)
    {
        Position = position;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    private void OnBodyEntered(Node2D body)
    {
        Hide(); // Player disappears after being hit.
        EmitSignal(SignalName.Hit);
        // Must be deferred as we can't change physics properties on a physics callback.
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }
}