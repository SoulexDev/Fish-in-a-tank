using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Fish : AnimatedSprite2D
{
    public enum FishState { Idle, Talking, Eating, Dead }
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
                        happyPlayer.Play();
                        break;
                    case FishState.Eating:
                        m_foodEaten = 0;
                        m_foods = FoodManager.Instance.GetNearbyFoods(Position);
                        vacuumPlayer.Play();
                        vacuumParticles.Emitting = true;
                        break;
                    case FishState.Dead:
                        GD.Print("dead as FUCK");
                        happyPlayer.Stop();
                        neutralPlayer.Stop();
                        depressedPlayer.Stop();
                        //nopePlayer.Stop();
                        vacuumPlayer.Stop();
                        vacuumParticles.Restart();
                        vacuumParticles.Emitting = false;
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

    [Export] public AudioStreamPlayer happyPlayer;
    [Export] public AudioStreamPlayer neutralPlayer;
    [Export] public AudioStreamPlayer depressedPlayer;

    [Export] public AudioStreamPlayer nopePlayer;

    [Export] public AudioStreamPlayer vacuumPlayer;

    [Export] public GpuParticles2D vacuumParticles;

    private int m_foodEaten;
    private float m_happiness;
    private Vector2 m_target;

    private List<RigidBody2D> m_foods;
    List<RigidBody2D> foodsToDelete = new List<RigidBody2D>();

    public override void _Ready()
    {
        base._Ready();

        m_happiness = 1;
        m_rand = new Random();
        m_speakingWatch = new Stopwatch();

        m_speakingWatch.Start();
        m_speakingInterval = m_rand.Next(10, 40);

        happyPlayer.Finished += OnPlayerFinished;
        neutralPlayer.Finished += OnPlayerFinished;
        depressedPlayer.Finished += OnPlayerFinished;
        vacuumPlayer.Finished += OnPlayerFinished;
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
        }
    }
    private void OnPlayerFinished()
    {
        if (m_currentState == FishState.Dead)
        {
            GD.Print("RETURNED as FUCK");
            return;
        }

        m_currentState = FishState.Idle;

        m_speakingWatch.Restart();
        m_speakingInterval = m_rand.Next(10, 40);

        vacuumParticles.Emitting = false;
    }
    public void DoIdle(double delta)
    {
        Play("Idle");
        if (Position.DistanceTo(m_target) > 1)
        {
            Position = Position.MoveToward(m_target, (float)delta * 100f);

            Vector2 direction = (m_target - Position).Normalized();
            FlipH = direction.X < 0;
        }
        else
            m_target = new Vector2((GD.Randf() - 0.5f) * 2 * 700, (GD.Randf() - 0.5f) * 2 * 350);

        if (m_speakingWatch.ElapsedMilliseconds * 0.001f >= m_speakingInterval)
        {
            m_currentState = FishState.Talking;
        }
        if (FoodManager.Instance.IsNearFood(Position))
        {
            m_currentState = FishState.Eating;
        }
    }
    public void DoTalking(double delta)
    {
        Play("Talking");
    }
    public void DoEating(double delta)
    {
        Play("Eating");
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta); 
        if (m_currentState == FishState.Eating)
        {
            for (int i = m_foods.Count - 1; i > 0 ; i--)
            {
                RigidBody2D f = m_foods[i];
                f.LinearVelocity = (Position - f.Position).Normalized() * 100;

                if (f.Position.DistanceTo(Position) <= 65)
                {
                    m_foods.Remove(f);
                    FoodManager.Instance.RemoveFood(f);

                    m_foodEaten++;
                }
            }
            if (m_foodEaten > 6)
                m_currentState = FishState.Dead;
        }
    }
    public void DoDead(double delta)
    {
        Play("Dead");
        FlipV = true;
        Position += Vector2.Up * (float)delta * 20f;
    }
}
