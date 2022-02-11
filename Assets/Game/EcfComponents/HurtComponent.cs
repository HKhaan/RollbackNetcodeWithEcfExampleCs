using FixedMath;
using RollBackExample;
using System;
[Serializable]
public struct HurtData
{
    public bool gotHurt;
    public bool isLeft;
    //this is for a test (should remove this before pushing)
}
public class HurtComponent : Component
{
    Entity entity;
    [RollBackProp]
    public HurtData Data;
    private AnimationComponent animationComponent = null;
    private MovementComponent movementComponent = null;
    public HurtComponent()
    {
    }

    public int OwnerId { get; set; }

    public int ExecuteIndex => 1;

    public void Update()
    {
        if (Data.gotHurt)
        {
            entity.body.force.y += Fix._1;
            if (Data.isLeft)
            {
                entity.body.force.x += Fix._1;
            }
            else
            {
                entity.body.force.x -= Fix._1;
            }
            Data.gotHurt = false;
            UnityEngine.Debug.Log("gothurt");
        }
    }

    public void Start(Entity owner)
    {
        entity = owner;
        owner.GetComponent(ref animationComponent);
        owner.GetComponent(ref movementComponent);
    }
}
