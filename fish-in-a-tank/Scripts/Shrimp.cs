using Godot;
using System;
using System.Diagnostics;
using static Fish;

public partial class Shrimp : RigidBody2D
{
    public bool beingEaten = false;

    private Stopwatch m_jumpTimer = new Stopwatch();
    private Stopwatch m_speakTimer = new Stopwatch();
    private Random m_rand = new Random();
    private int m_jumpInterval;
    private int m_speakInterval;

    [Export] public Sprite2D sprite;
    [Export] public CollisionObject2D collider;

    [Export] public AudioStreamPlayer2D audioPlayer;
    [Export] public AudioStreamPlayer2D screamPlayer;
    [Export] public PackedScene blood;

    public override void _Ready()
    {
        base._Ready();
        m_jumpTimer.Start();
        m_speakTimer.Start();
        m_jumpInterval = m_rand.Next(750, 1250);
        m_speakInterval = m_rand.Next(8000, 16000);

        collider.InputEvent += Collider_InputEvent;
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (!beingEaten && m_jumpTimer.ElapsedMilliseconds >= m_jumpInterval)
        {
            m_jumpTimer.Restart();
            m_jumpInterval = m_rand.Next(750, 1250);

            ApplyCentralImpulse(Vector2.Up * 100 + Vector2.Right * (GD.Randf() * 2 - 1) * 250);
        }
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        sprite.FlipH = LinearVelocity.X > 0;

        if (!beingEaten && m_speakTimer.ElapsedMilliseconds >= m_speakInterval)
        {
            m_speakTimer.Restart();
            m_speakInterval = m_rand.Next(8000, 16000);

            audioPlayer.Play();
        }
    }
    private void Collider_InputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseBtn)
        {
            if (mouseBtn.Pressed && mouseBtn.ButtonIndex == MouseButton.Left)
            {
                if (InteractionManager.Instance.selectedItem == InteractionManager.Item.Glock)
                    ExplodeFish();
            }
        }
    }
    public void ExplodeFish()
    {
        GpuParticles2D newBlood = (GpuParticles2D)GD.Load<PackedScene>(blood.ResourcePath).Instantiate();
        GetParent().AddChild(newBlood);
        newBlood.Position = GetGlobalMousePosition();
        newBlood.Emitting = true;

        InteractionManager.Instance.RemoveShrimp(this);
    }
    public void StartEat()
    {
        screamPlayer.Play();

        beingEaten = true;
    }
    public void EndEat()
    {
        beingEaten = false;
    }
}
