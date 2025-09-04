using Godot;
using System;

public partial class Bomb : StaticBody2D
{
    [Export] public PackedScene explosionParticles;
    [Export] public CollisionObject2D collider;

    public override void _Ready()
    {
        base._Ready();

        collider.InputEvent += Collider_InputEvent;
    }
    public void Explode()
    {
        GpuParticles2D newExplosion = (GpuParticles2D)GD.Load<PackedScene>(explosionParticles.ResourcePath).Instantiate();
        GetParent().AddChild(newExplosion);
        newExplosion.Position = Position;
        newExplosion.Emitting = true;

        InteractionManager.Instance.RemoveBomb(this);
    }
    private void Collider_InputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseBtn)
        {
            if (mouseBtn.Pressed && mouseBtn.ButtonIndex == MouseButton.Left)
            {
                if (InteractionManager.Instance.selectedItem == InteractionManager.Item.Glock)
                    Explode();
            }
        }
    }
}
