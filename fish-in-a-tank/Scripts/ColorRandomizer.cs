using Godot;
using System;

public partial class ColorRandomizer : Sprite2D
{
    public override void _Ready()
    {
        base._Ready();
        Modulate = new Color(GD.Randf() * 0.5f + 0.4f, GD.Randf() * 0.5f + 0.4f, GD.Randf() * 0.5f + 0.4f);
    }
}
