using Godot;
using System;
using System.Collections.Generic;

public partial class InteractionManager : Node2D
{
    public static InteractionManager Instance;
    public enum Item { Clicker, Food, Rock, Shrimp, HelenFish, LickFish, Glock, SandCastle, Plant, Bomb }
    public Item selectedItem = Item.Clicker;

    [Export] public PackedScene food;
    [Export] public PackedScene rock;
    [Export] public PackedScene shrimp;
    [Export] public PackedScene helenFish;
    [Export] public PackedScene lickFish;
    [Export] public PackedScene sandCastle;
    [Export] public PackedScene plant;
    [Export] public PackedScene bomb;

    [Export] public Sprite2D glock;
    [Export] public Sprite2D crosshair;

    [Export] public AudioStreamPlayer gunFirePlayer;

    private List<RigidBody2D> foods = new List<RigidBody2D>();
    private Vector2 glockPos;

    public Dictionary<Item, int> remainingValues = new Dictionary<Item, int>
    {
        { Item.Clicker, 0 },
        { Item.Food, 0 },
        { Item.Rock, 3 },
        { Item.Shrimp, 4 },
        { Item.HelenFish, 1 },
        { Item.LickFish, 1 },
        { Item.SandCastle, 2 },
        { Item.Plant, 4 },
        { Item.Glock, 0 },
        { Item.Bomb, 4 }
    };
    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        Input.SetDefaultCursorShape(Input.CursorShape.PointingHand);
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
                case Item.HelenFish:
                    OnHelenClick();
                    break;
                case Item.LickFish:
                    OnLickClick();
                    break;
                case Item.SandCastle:
                    break;
                case Item.Plant:
                    break;
                case Item.Glock:
                    OnGlockClick();
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

        crosshair.Position = GetGlobalMousePosition();
    }
    public void SelectItem(Item item)
    {
        selectedItem = item;
        glock.Visible = false;
        crosshair.Visible = false;

        Input.MouseMode = Input.MouseModeEnum.Visible;

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
            case Item.HelenFish:
                break;
            case Item.LickFish:
                break;
            case Item.SandCastle:
                break;
            case Item.Plant:
                break;
            case Item.Glock:
                glock.Visible = true;
                crosshair.Visible = true;
                Input.MouseMode = Input.MouseModeEnum.Hidden;
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

        Vector2 spawnPos = GetGlobalMousePosition();
        spawnPos.X = Mathf.Clamp(spawnPos.X, -700, 700);
        spawnPos.Y = Mathf.Clamp(spawnPos.Y, -200, 300);

        ((RigidBody2D)newFood).Position = spawnPos;
    }
    void OnRockClick()
    {
        if (remainingValues[Item.Rock] <= 0)
        {

            return;
        }
        var newRock = GD.Load<PackedScene>(rock.ResourcePath).Instantiate();
        AddChild(newRock);

        Vector2 spawnPos = GetGlobalMousePosition();
        spawnPos.X = Mathf.Clamp(spawnPos.X, -700, 700);
        spawnPos.Y = Mathf.Clamp(spawnPos.Y, -200, 340);

        ((RigidBody2D)newRock).Position = spawnPos;

        remainingValues[Item.Rock]--;
    }
    void OnGlockClick()
    {
        gunFirePlayer.Play();
    }
    void OnHelenClick()
    {
        if (remainingValues[Item.HelenFish] <= 0)
        {

            return;
        }
        var newFish = GD.Load<PackedScene>(helenFish.ResourcePath).Instantiate();
        AddChild(newFish);

        Vector2 spawnPos = GetGlobalMousePosition();
        spawnPos.X = Mathf.Clamp(spawnPos.X, -700, 700);
        spawnPos.Y = Mathf.Clamp(spawnPos.Y, -200, 340);

        ((CharacterBody2D)newFish).Position = spawnPos;

        remainingValues[Item.HelenFish]--;
    }
    void OnLickClick()
    {
        if (remainingValues[Item.LickFish] <= 0)
        {

            return;
        }
        var newFish = GD.Load<PackedScene>(lickFish.ResourcePath).Instantiate();
        AddChild(newFish);

        Vector2 spawnPos = GetGlobalMousePosition();
        spawnPos.X = Mathf.Clamp(spawnPos.X, -700, 700);
        spawnPos.Y = Mathf.Clamp(spawnPos.Y, -200, 340);

        ((CharacterBody2D)newFish).Position = spawnPos;

        remainingValues[Item.LickFish]--;
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
    public void RemoveBomb(StaticBody2D bombToRemove)
    {
        bombToRemove.QueueFree();

        remainingValues[Item.Bomb]++;
    }
    public void RemoveShrimp(RigidBody2D shrimpToRemove)
    {
        foods.Remove(shrimpToRemove);
        shrimpToRemove.QueueFree();

        remainingValues[Item.Shrimp]++;
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
    public void _on_helen_btn_pressed()
    {
        SelectItem(Item.HelenFish);
    }
    public void _on_lick_btn_pressed()
    {
        SelectItem(Item.LickFish);
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
