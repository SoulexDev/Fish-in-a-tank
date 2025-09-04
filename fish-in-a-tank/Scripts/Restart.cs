using Godot;
using System;

public partial class Restart : Node2D
{
    public void _on_restart_pressed()
    {
        GetTree().ReloadCurrentScene();
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }
}
