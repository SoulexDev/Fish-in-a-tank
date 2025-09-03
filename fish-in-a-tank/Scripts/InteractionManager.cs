using Godot;
using System;
using System.Collections.Generic;

public partial class InteractionManager : Node2D
{
    public static InteractionManager Instance;
    public enum Item { Clicker, Food, Rock, Shrimp, Mermaid, OtherFish, Glock, SandCastle, Plant, Bomb }
    public Item selectedItem = Item.Clicker;

    [Export] public PackedScene food;
    [Export] public Sprite2D glock;

    private List<RigidBody2D> foods = new List<RigidBody2D>();
    private Vector2 glockPos;
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
            switch (selectedItem)
            {
                case Item.Clicker:
                    break;
                case Item.Food:
                    OnFoodClick();
                    break;
                case Item.Rock:
                    break;
                case Item.Shrimp:
                    break;
                case Item.Mermaid:
                    break;
                case Item.OtherFish:
                    break;
                case Item.SandCastle:
                    break;
                case Item.Plant:
                    break;
                case Item.Glock:
                    break;
                case Item.Bomb:
                    break;
                default:
                    break;
            }
            
        }

        Vector2 desiredGlockPos = GetGlobalMousePosition() * 0.1f;

        desiredGlockPos.X = Mathf.Clamp(desiredGlockPos.X, -500, 500);
        desiredGlockPos.Y = Mathf.Clamp(desiredGlockPos.Y, -100, 400);
        desiredGlockPos.Y += 400;

        glockPos = glockPos.Lerp(desiredGlockPos, (float)delta * 5);

        glock.Position = glockPos;
    }
    public void SelectItem(Item item)
    {
        selectedItem = item;
        glock.Visible = false;

        GD.Print(item);
        switch (selectedItem)
        {
            case Item.Clicker:
                break;
            case Item.Food:
                break;
            case Item.Rock:
                break;
            case Item.Shrimp:
                break;
            case Item.Mermaid:
                break;
            case Item.OtherFish:
                break;
            case Item.SandCastle:
                break;
            case Item.Plant:
                break;
            case Item.Glock:
                glock.Visible = true;
                break;
            case Item.Bomb:
                break;
            default:
                break;
        }
    }
    void OnFoodClick()
    {
        var newFood = GD.Load<PackedScene>(food.ResourcePath).Instantiate();
        AddChild(newFood);

        foods.Add((RigidBody2D)newFood);

        ((RigidBody2D)newFood).Position = GetGlobalMousePosition();
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
    public void _on_clicker_btn_pressed()
    {
        SelectItem(Item.Clicker);
    }
    public void _on_food_btn_pressed()
    {
        SelectItem(Item.Food);
    }
    public void _on_rock_btn_pressed()
    {
        SelectItem(Item.Rock);
    }
    public void _on_shrimp_btn_pressed()
    {
        SelectItem(Item.Shrimp);
    }
    public void _on_mermaind_btn_pressed()
    {
        SelectItem(Item.Mermaid);
    }
    public void _on_other_fish_btn_pressed()
    {
        SelectItem(Item.OtherFish);
    }
    public void _on_plant_btn_pressed()
    {
        SelectItem(Item.Plant);
    }
    public void _on_sand_castle_btn_pressed()
    {
        SelectItem(Item.SandCastle);
    }
    public void _on_glock_btn_pressed()
    {
        SelectItem(Item.Glock);
    }
    public void _on_bomb_btn_pressed()
    {
        SelectItem(Item.Bomb);
    }
}
