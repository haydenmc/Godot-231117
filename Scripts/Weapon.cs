using Godot;
using System;

public partial class Weapon : Node3D
{
    [Export]
    public PackedScene Projectile { get; set; }

    public void Fire(Vector3 target)
    {
        var projectile = Projectile.Instantiate<LaserBoltProjectile>();
        projectile.Position = GetNode<Node3D>("Muzzle").GlobalPosition;
        projectile.Target = target;
        GetNode("/root/World").AddChild(projectile);
    }
}
