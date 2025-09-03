using Godot;
using System;
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
                switch (_m_currentState)
                {
                    case FishState.Idle:
                        break;
                    case FishState.Talking:
                        happyPlayer.Play();
                        break;
                    case FishState.Eating:
                        break;
                    case FishState.Dead:
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

    private float m_happiness;
    private Vector2 m_target;

    public override void _Ready()
    {
        base._Ready();

        m_happiness = 1;
        m_rand = new Random();
        m_speakingWatch = new Stopwatch();

        m_speakingWatch.Start();
        m_speakingInterval = m_rand.Next(25, 80);

        happyPlayer.Finished += OnPlayerFinished;
        neutralPlayer.Finished += OnPlayerFinished;
        depressedPlayer.Finished += OnPlayerFinished;
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
        m_currentState = FishState.Idle;

        m_speakingWatch.Restart();
        m_speakingInterval = m_rand.Next(25, 80);
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
    }
    public void DoTalking(double delta)
    {
        Play("Talking");
    }
    public void DoEating(double delta)
    {
        Play("Eating");
    }
    public void DoDead(double delta)
    {
        Play("Dead");
        FlipV = true;
        Position += Vector2.Up * (float)delta * 20f;
    }
}
