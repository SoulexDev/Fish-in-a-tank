using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Fish : CharacterBody2D
{
    public enum FishState { Idle, Talking, Eating, Dead, Denying }
    private FishState _m_currentState = FishState.Idle;
    private FishState m_currentState
    {
        get { return _m_currentState; }
        set
        {
            if(value != _m_currentState)
            {
                _m_currentState = value;
                GD.Print(value);
                GD.Print(_m_currentState);
                switch (_m_currentState)
                {
                    case FishState.Idle:
                        break;
                    case FishState.Talking:
                        if (m_happiness > 66)
                            happyPlayer.Play();
                        else if (m_happiness > 33)
                            neutralPlayer.Play();
                        else
                            depressedPlayer.Play();
                        break;
                    case FishState.Eating:
                        m_foodEaten = 0;
                        m_foods = InteractionManager.Instance.GetNearbyFoods(Position);
                        vacuumPlayer.Play();
                        vacuumParticles.Emitting = true;

                        m_foods.ForEach(f =>
                        {
                            if (f is Shrimp)
                            {
                                (f as Shrimp).StartEat();
                            }
                        });
                        break;
                    case FishState.Dead:
                        GD.Print("dead as FUCK");
                        happyPlayer.Stop();
                        neutralPlayer.Stop();
                        depressedPlayer.Stop();
                        nopePlayer.Stop();
                        vacuumPlayer.Stop();
                        vacuumParticles.Restart();
                        vacuumParticles.Emitting = false;
                        break;
                    case FishState.Denying:
                        nopePlayer.Play();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private Stopwatch m_speakingWatch;
    private float m_speakingInterval;
    private Random m_rand;

    [Export] public AnimatedSprite2D fishSprite;
    [Export] public CollisionObject2D collider;

    [Export] public AudioStreamPlayer2D happyPlayer;
    [Export] public AudioStreamPlayer2D neutralPlayer;
    [Export] public AudioStreamPlayer2D depressedPlayer;
    [Export] public AudioStreamPlayer2D nopePlayer;

    [Export] public AudioStreamPlayer2D vacuumPlayer;

    [Export] public GpuParticles2D vacuumParticles;

    [Export] public ProgressBar happinessBar;

    [Export] public PackedScene blood;

    [Export] public bool otherFish;

    private int m_foodEaten;

    private float _m_happiness;
    private float m_happiness
    {
        get { return _m_happiness; }
        set
        {
            _m_happiness = Mathf.Clamp(value, 0, 100 - m_age);
            happinessBar.Value = m_happiness;
        }
    }
    private float m_age;
    private string m_ageTag
    {
        get
        {
            if (otherFish)
                return "";
            if (m_age > 66)
                return "OLD";
            else if (m_age > 33)
                return "MA";
            else
                return "";
        }
    }

    private Vector2 m_target;

    private List<RigidBody2D> m_foods;
    List<RigidBody2D> foodsToDelete = new List<RigidBody2D>();

    public override void _Ready()
    {
        base._Ready();

        m_happiness = 100;
        m_rand = new Random();
        m_speakingWatch = new Stopwatch();

        m_speakingWatch.Start();
        m_speakingInterval = m_rand.Next(10, 40);

        happyPlayer.Finished += OnPlayerFinished;
        neutralPlayer.Finished += OnPlayerFinished;
        depressedPlayer.Finished += OnPlayerFinished;
        vacuumPlayer.Finished += OnPlayerFinished;
        nopePlayer.Finished += OnPlayerFinished;

        collider.InputEvent += Collider_InputEvent;
    }

    private void Collider_InputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseBtn)
        {
            if (mouseBtn.Pressed && mouseBtn.ButtonIndex == MouseButton.Left)
            {
                if (InteractionManager.Instance.selectedItem == InteractionManager.Item.Clicker)
                    m_currentState = FishState.Denying;
                else if (InteractionManager.Instance.selectedItem == InteractionManager.Item.Glock)
                    ExplodeFish();
            }
        }
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        switch (m_currentState)
        {
            case FishState.Idle:
                DoIdle(delta);
                break;
            case FishState.Talking:
                DoTalking(delta);
                break;
            case FishState.Eating:
                DoEating(delta);
                break;
            case FishState.Dead:
                DoDead(delta);
                break;
            case FishState.Denying:
                DoTalking(delta);
                break;
            default:
                break;
        }

        m_happiness -= (float)delta * 0.5f;
        m_age += (float)delta * 0.1f;
        m_age = Mathf.Clamp(m_age, 0, 90);

        if (m_age >= 90)
            m_currentState = FishState.Dead;

        StaticBody2D bomb = InteractionManager.Instance.GetNearbyBomb(Position);
        if (bomb != null)
        {
            (bomb as Bomb).Explode();
            ExplodeFish(true);
        }
    }
    private void OnPlayerFinished()
    {
        if (m_currentState == FishState.Dead)
            return;
        if (m_currentState != FishState.Talking || m_currentState != FishState.Denying)
        {
            m_speakingWatch.Restart();
            m_speakingInterval = m_rand.Next(10, 40);
        }
        if (m_currentState == FishState.Eating)
        {
            if (m_foods.Count > 0)
            {
                m_foods.ForEach(f =>
                {
                    if (f is Shrimp)
                    {
                        (f as Shrimp).EndEat();
                    }
                });
            }
        }

        m_currentState = FishState.Idle;
        vacuumParticles.Emitting = false;
    }
    public void ExplodeFish(bool vanish = false)
    {
        GpuParticles2D newBlood = (GpuParticles2D)GD.Load<PackedScene>(blood.ResourcePath).Instantiate();
        GetParent().AddChild(newBlood);
        newBlood.Position = Position;
        newBlood.Emitting = true;

        if (vanish)
            QueueFree();
        else
            m_currentState = FishState.Dead;
    }
    public void DoIdle(double delta)
    {
        fishSprite.Play("Idle" + m_ageTag);
        if (Position.DistanceTo(m_target) > 1)
        {
            Position = Position.MoveToward(m_target, (float)delta * 100f);

            Vector2 direction = (m_target - Position).Normalized();
            fishSprite.FlipH = direction.X < 0;
        }
        else
            m_target = new Vector2((GD.Randf() - 0.5f) * 2 * 700, (GD.Randf() - 0.5f) * 2 * 350);

        if (m_speakingWatch.ElapsedMilliseconds * 0.001f >= m_speakingInterval)
        {
            m_currentState = FishState.Talking;
        }
        if (InteractionManager.Instance.IsNearFood(Position))
        {
            m_currentState = FishState.Eating;
        }
    }
    public void DoTalking(double delta)
    {
        fishSprite.Play("Talking" + m_ageTag);
    }
    public void DoEating(double delta)
    {
        fishSprite.Play("Eating" + m_ageTag);
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta); 
        if (m_currentState == FishState.Eating)
        {
            for (int i = m_foods.Count - 1; i > 0 ; i--)
            {
                RigidBody2D f = m_foods[i];

                if (f == null)
                    continue;
                f.LinearVelocity = (Position - f.Position).Normalized() * 100;

                if (f.Position.DistanceTo(Position) <= 65)
                {
                    m_foods.Remove(f);

                    if (f is Shrimp)
                        InteractionManager.Instance.RemoveShrimp(f);
                    else
                        InteractionManager.Instance.RemoveFood(f);

                    m_foodEaten++;

                    m_happiness += 5;
                }
            }
            if (m_foodEaten > 6)
                m_currentState = FishState.Dead;
        }
    }
    public void DoDead(double delta)
    {
        fishSprite.Play("Dead" + m_ageTag);
        fishSprite.FlipV = true;
        Position += Vector2.Up * (float)delta * 20f;
    }
}
