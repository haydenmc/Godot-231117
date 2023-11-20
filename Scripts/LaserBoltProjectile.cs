using Godot;
using System;

public partial class LaserBoltProjectile : Node3D
{
    [Export]
    public Vector3 Target { get; set; }

    [Export]
    public float ProjectileSpeed { get; set; } = 200.0f;

    [Export]
    public float MaximumRange { get; set; } = 200.0f;
    
    private float _MaximumRangeSquared { get; set; }

    private Vector3 _OriginPosition { get; set; }

    private Vector3 _TargetDirection { get; set; }

    public override void _Ready()
    {
        base._Ready();
        _OriginPosition = GlobalPosition;
        _TargetDirection = GlobalPosition.DirectionTo(Target);
        _MaximumRangeSquared = (float)Math.Pow(MaximumRange, 2);
        LookAt(Target);
    }

    public override void _PhysicsProcess(double delta)
    {
        var newPosition = (Position + (_TargetDirection * (ProjectileSpeed * (float)delta)));
        Position = newPosition;

        if (_OriginPosition.DistanceSquaredTo(GlobalPosition) > _MaximumRangeSquared)
        {
            GetParent().RemoveChild(this);
            QueueFree();
        }
        base._PhysicsProcess(delta);
    }
}
