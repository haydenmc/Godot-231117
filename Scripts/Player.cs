using Godot;
using System;

public partial class Player : CharacterBody3D, IControllable
{
    [Export]
    public float MovementAcceleration = 150.0f;

    [Export]
    public float MovementDeceleration = 10.0f;

    [Export]
    public float JumpVelocity = 15.0f;

    [Export]
    public float GravityMultiplier = 4.0f;

    [Export]
    public float CameraXSensitivity = 0.005f;

    [Export]
    public float CameraYSensitivity = 0.005f;

    [Export]
    public PackedScene WeaponScene { get; set; }
    
    private Vector2 _movementDirection;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    private Node3D _head;
    private Camera3D _camera;
    private AnimationPlayer _bobAnimation;
    private RayCast3D _reticleRaycast;
    private Node3D _weaponHand;
    private Weapon _weapon;
    private bool _isCameraActive = false;

    public override void _Ready()
    {
        _head = GetNode<Node3D>("Head");
        _camera = GetNode<Camera3D>("Head/Camera");
        if (_isCameraActive)
        {
            _camera.MakeCurrent();
        }
        _bobAnimation = GetNode<AnimationPlayer>("Head/Camera/BobAnimation");
        _reticleRaycast = GetNode<RayCast3D>("Head/Camera/ReticleRaycast");

        _weaponHand = GetNode<Node3D>("Head/WeaponHand");
        _weapon = WeaponScene.Instantiate<Weapon>();
        _weaponHand.AddChild(_weapon);
    }

    public override void _Input(InputEvent @event)
    { }

    public override void _PhysicsProcess(double delta)
    {
        if (_movementDirection.Length() > 0)
        {
            _bobAnimation.Play("view_bob");
        }
        else
        {
            _bobAnimation.Play("RESET");
        }
        // Determine movement
        Vector3 direction = (Transform.Basis *
            new Vector3(_movementDirection.X, 0, _movementDirection.Y))
            .Normalized();
        var acceleration = direction * (float)(MovementAcceleration * delta);
        var drag = -Velocity * new Vector3(1, 0, 1) * (float)(MovementDeceleration * delta);
        var newVelocity = (Velocity + (acceleration + drag));

        // Add the gravity.
        if (!IsOnFloor())
        {
            newVelocity.Y -= gravity * GravityMultiplier * (float)delta;
        }
        Velocity = newVelocity;
        MoveAndSlide();
    }

    public void SetCamera(bool isCurrent)
    {
        _isCameraActive = isCurrent;
        if (_camera != null)
        {
            if (_isCameraActive)
            {
                _camera.MakeCurrent();
            }
            else
            {
                _camera.ClearCurrent(false);
            }
        }
    }

    public void SetMovement(Vector2 direction)
    {
        _movementDirection = direction;
    }

    public void RotateAim(Vector2 delta)
    {
        RotateY(-delta.X * CameraXSensitivity);
        _head.RotateX(-delta.Y * CameraYSensitivity);
        _head.Rotation = _head.Rotation with {
            X = Mathf.Clamp(_head.Rotation.X, -Mathf.Pi/2.0f, Mathf.Pi/2.0f) };
    }

    public void Jump()
    {
        if (IsOnFloor())
        {
            Velocity = Velocity with { Y = JumpVelocity };
        }
    }

    public void Fire()
    {
        if (_reticleRaycast.IsColliding())
        {
            _weapon.Fire(_reticleRaycast.GetCollisionPoint());
        }
        else
        {
            _weapon.Fire(_reticleRaycast.ToGlobal(_reticleRaycast.TargetPosition));
        }
    }
}
