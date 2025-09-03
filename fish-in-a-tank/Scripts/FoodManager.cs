using Godot;
using System;
using System.Collections.Generic;

public partial class FoodManager : Node2D
{
    public static FoodManager Instance;

    [Export] public PackedScene food;

    private List<RigidBody2D> foods = new List<RigidBody2D>();

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("MouseClick"))
        {
            var newFood = GD.Load<PackedScene>(food.ResourcePath).Instantiate();
            AddChild(newFood);

            foods.Add((RigidBody2D)newFood);

            ((RigidBody2D)newFood).Position = GetGlobalMousePosition();
        }
    }
    public List<RigidBody2D> GetNearbyFoods(Vector2 position)
    {
        List<RigidBody2D> nearbyFoods = new List<RigidBody2D>();
        foreach (var f in foods)
        {
            if (position.DistanceSquaredTo(f.Position) < 200 * 200)
                nearbyFoods.Add(f);
        }
        return nearbyFoods;
    }
    public bool IsNearFood(Vector2 position)
    {
        int counter = 0;
        foreach (var f in foods)
        {
            if (position.DistanceSquaredTo(f.Position) < 150 * 150)
            {
                counter++;
                if (counter > 3)
                    return true;
            }
        }
        return false;
    }
    public void RemoveFood(RigidBody2D foodToRemove)
    {
        foods.Remove(foodToRemove);
        foodToRemove.QueueFree();
    }
}
