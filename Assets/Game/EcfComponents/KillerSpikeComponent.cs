using FixedMath;
using RollBackExample;
using System;


public class KillerSpikeComponent : Component
{
    Entity entity;
    public KillerSpikeComponent()
    {
    }

    public int OwnerId { get; set; }

    public int ExecuteIndex => 2;
    public void Start(Entity owner)
    {
        entity = owner;
        owner.body.CollidedTop.Add((other) =>
        {
            MovementComponent mc = null;
            other.GetComponent(ref mc);
            if (mc != null) {
                other.body.position.x = mc.Data.lastCheckpoint.x;
                other.body.position.y = mc.Data.lastCheckpoint.y;
            }

        });
    }
    public void Update()
    {

    }

}
