
using FixedMath;
using RollBackExample;
using System;
using System.Collections.Generic;

public class Body : Component
{
    public int ExecuteIndex => 999;

    public int OwnerId { get; set; }
    public Entity Owner { get; set; }

    [RollBackProp]
    public FixVector force;

    [RollBackProp]
    public FixVector position;
    [RollBackProp]
    public Fix gravity;
    [RollBackProp]
    public bool grounded;
    [RollBackProp]
    public bool enabled;

    public void Update()
    {
        
        //Owner.body.force.y += FixedMath.Fix._0_10 * FixedMath.Fix.minus_one;
    }

    public virtual bool Intersects(Body toCompare) {
        throw new NotImplementedException();
    }

    public virtual void Start(Entity owner)
    {
        owner.body.CollidedBottom.Add((Entity other) => {
            grounded = true;
            
        });
    }

    public List<Action<Entity>> CollidedTop = new List<Action<Entity>>();
    public List<Action<Entity>> CollidedBottom = new List<Action<Entity>>();
    public List<Action<Entity>> CollidedRight = new List<Action<Entity>>();
    public List<Action<Entity>> CollidedLeft = new List<Action<Entity>>();
}
