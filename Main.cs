using Godot;
using System;
using System.Linq;
using First2DGame.Extensions;

public partial class Main : Node
{
    [Export] public PackedScene MobScene { get; set; }

    [Export] public PackedScene TrackingMobScene { get; set; }

    [Export] public PackedScene PlayerProjectileScene { get; set; }

    [Export] public PackedScene AmmoScene { get; set; }

    [Export] public int TrackingMobSpawnScoreDivider { get; set; } = 25;

    [Export] public int AmmoSpawnScoreDivider { get; set; } = 17;

    private Vector2 _screenSize; // Size of the game window.
    private float _playerCollisionShapeRadius;

    private int _score;

    private bool _gameActive;

    public void NewGame()
    {
        _score = 0;
        var hud = GetNode<Hud>("HUD");
        hud.UpdateScore(_score);
        hud.ShowMessage("Get ready...");

        var player = GetNode<Player>("Player");
        var startPosition = GetNode<Marker2D>("StartPosition");
        player.Start(startPosition.Position);

        var startTimer = GetNode<Timer>("StartTimer");
        startTimer.Start();
        GetNode<AudioStreamPlayer>("Music").Play();
        _gameActive = true;
    }

    public override void _Ready()
    {
        _screenSize = GetViewport().GetVisibleRect().Size;
        
        var collisionShape = GetNode<CollisionShape2D>("Player/CollisionShape2D");
        _playerCollisionShapeRadius = (collisionShape.Shape as CapsuleShape2D)!.Radius;
    }

    public override void _Input(InputEvent @event)
    {
        if (_gameActive && @event is InputEventMouseButton { Pressed: true } eventMouseButton)
        {
            var player = GetNode<Player>("Player");
            if (player.TryShoot())
            {
                var eye = GetNode<TextureRect>("Player/Eye");
                var eyeCenter = eye.GlobalPosition + eye.PivotOffset.Rotated(eye.Rotation) * eye.Scale;
                var newAngle = eventMouseButton.GlobalPosition - eyeCenter;

                var projectile = PlayerProjectileScene.Instantiate<PlayerProjectile>();

                projectile.Position = eyeCenter + new Vector2(_playerCollisionShapeRadius, 0).Rotated(eye.Rotation);
                projectile.Rotation = newAngle.Angle() + float.Pi / 2;
                projectile.LinearVelocity = newAngle.Normalized() * projectile.Speed;

                AddChild(projectile);
            }
        }
    }

    private void GameOver()
    {
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();

        var hud = GetNode<Hud>("HUD");
        hud.ShowGameOver(_score);

        GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<AudioStreamPlayer>("DeathSound").Play();

        GetTree().CallGroup("Mobs", Node.MethodName.QueueFree);
        GetTree().CallGroup("Projectiles", Node.MethodName.QueueFree);
        GetTree().CallGroup("Ammo", Node.MethodName.QueueFree);
        _gameActive = false;
    }

    private void OnMobTimerTimeout()
    {
        SpawnRegularMob();

        var trackingMob = TryGetTrackingMob();
        if (_score % TrackingMobSpawnScoreDivider == 0 && _score > 0 && trackingMob is null)
        {
            SpawnTrackingMob();
        }
    }

    private TrackingMob? TryGetTrackingMob()
    {
        var trackingMob = GetTree().GetFirstNodeInGroup("TrackingMob");
        return trackingMob as TrackingMob;
    }

    private void SpawnTrackingMob()
    {
        var trackingMob = TrackingMobScene.Instantiate<TrackingMob>();

        var mobSpawnLocation = GetSpawnLocation();
        trackingMob.Position = mobSpawnLocation.Position;

        AddChild(trackingMob);
    }

    private void SpawnRegularMob()
    {
        var newMob = MobScene.Instantiate<Mob>();
        var mobSpawnLocation = GetSpawnLocation();

        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
        var direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

        // Add some randomness to the direction.
        const double maxDirectionRotation = Mathf.Pi / 4;
        const double minDirectionRotation = -Mathf.Pi / 4;
        direction += (float)GD.RandRange(minDirectionRotation, maxDirectionRotation);

        newMob.Position = mobSpawnLocation.Position;
        newMob.Rotation = direction;
        newMob.LinearVelocity = velocity.Rotated(direction);

        AddChild(newMob);
    }

    private PathFollow2D GetSpawnLocation()
    {
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf();

        return mobSpawnLocation;
    }

    private void OnScoreTimerTimeout()
    {
        _score++;
        GetNode<Hud>("HUD").UpdateScore(_score);

        if (_score % AmmoSpawnScoreDivider == 0 && !(GetTree().GetNodeCountInGroup("Ammo") > 0))
        {
            SpawnAmmo();
        }
    }

    private void SpawnAmmo()
    {
        const double offset = 15;
        var ammo = AmmoScene.Instantiate<Ammo>();
        var collisionShape = ammo.GetNode<CollisionShape2D>("CollisionShape2D");
        ammo.Position = new Vector2(
            (float)GD.RandRange(offset, _screenSize.X - (collisionShape.Shape as RectangleShape2D)!.Size.X * collisionShape.Scale.X),
            (float)GD.RandRange(offset, _screenSize.Y - (collisionShape.Shape as RectangleShape2D)!.Size.Y * collisionShape.Scale.Y));

        var firstMobIndex = GetChildren().FindIndex(n => n.IsInGroup("Mobs"));

        AddChild(ammo);
        MoveChild(ammo, firstMobIndex);
    }

    private void OnStartTimerTimeout()
    {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }
}