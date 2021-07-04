using FixedMath;
using RollBackExample;
using System;

public class GameStateComponent : Component
{
    Entity entity;
    [RollBackProp]
    public int CurrentFrame;
    public GameStateComponent()
    {
    }

    public int OwnerId { get; set; }

    public int ExecuteIndex => 1;
    public void Start(Entity owner)
    {

    }
    public void Update()
    {
        CurrentFrame++;
    }
}
