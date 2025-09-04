using Godot;
using System;

public partial class Rock : ShapeCast2D
{
    [Export] public RigidBody2D rigidBody;
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        //if (rigidBody.LinearVelocity.Y <= 0)
        //    return;

        for (int i = 0; i < GetCollisionCount(); i++)
        {
            GodotObject obj = GetCollider(i);

            Node parent = (obj as Node).GetParent();

            if (parent is Fish)
            {
                (parent as Fish).ExplodeFish(true);
            }
        }
    }
}
