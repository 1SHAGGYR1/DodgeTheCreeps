using Godot;
using System;

public partial class Main : Node
{
    [Export] public PackedScene MobScene { get; set; }

    [Export] public PackedScene BossMobScene { get; set; }

    private int _score;

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
    }

    private void OnMobTimerTimeout()
    {
        // SpawnRegularMob();

        var mobBoss = TryGetMobBoss();
        if (mobBoss is not null)
        {
            var player = GetNode<Player>("Player");
            mobBoss.Recalibrate(player.Position);
        }
        else if (_score % 5 == 0 && _score > 0)
        {
            SpawnBossMob();
        }
    }

    private BossMob? TryGetMobBoss()
    {
        var bossMob = GetTree().GetFirstNodeInGroup("BossMob");
        return bossMob as BossMob;
    }

    private void SpawnBossMob()
    {
        var bossMob = BossMobScene.Instantiate<BossMob>();
        
        var mobSpawnLocation = GetSpawnLocation();
        bossMob.Position = mobSpawnLocation.Position;
        
        var player = GetNode<Player>("Player");
        bossMob.Recalibrate(player.Position);
        AddChild(bossMob);
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
    }

    private void OnStartTimerTimeout()
    {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }
}