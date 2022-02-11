using FixedMath;
using RollBackExample;
using System;
[Serializable]
public struct MovementData
{
    public FixVector accelaration;
    public bool jumping;
    public bool jumpOverride;
    public bool lookingLeft;
    public FixVector jumpOverrideVal;
    public FixVector lastCheckpoint;
    //this is for a test (should remove this before pushing)
}
public class MovementComponent : Component
{
    Entity entity;
    [RollBackProp]
    public MovementData Data;
    public Character character;
    private HitComponent hitComponent = null;
    public MovementComponent()
    {
    }

    public int OwnerId { get; set; }

    public int ExecuteIndex => 1;
    public void Start(Entity owner)
    {
        entity = owner;
        owner.body.CollidedBottom.Add((other) =>
        {
            Data.jumping = false;
            entity.body.gravity = Fix._0;
            Data.jumpOverride = false;
        });
        owner.body.CollidedTop.Add((other) =>
        {
            Data.jumpOverride = false;
            Data.jumping = false;
            entity.body.gravity = Fix._0;
        });
        entity.GetComponent(ref hitComponent);
    }
    public void Update()
    {
        if (hitComponent.Data.hitting)
        {
            return;
        }
        Accelaration();
        JumpLogic();
        entity.body.force += Data.accelaration;
        Physics();
    }

    private void JumpLogic()
    {
        bool jumpPressed = false;
        if (entity.InputPressed(EInputTypes.jump))
        {
            if (entity.body.grounded)
                Data.jumping = true;
            jumpPressed = true;
        }
        if (Data.jumping)
        {
            Data.accelaration.y = Fix._0_20 + Fix._0_20;
            if (Data.jumpOverride)
            {
                Data.accelaration += Data.jumpOverrideVal;
            }
            else if (jumpPressed)
            {
                Data.accelaration.y += Fix._0_10 + Fix._0_05;
            }
            if (entity.body.gravity > Fix._1)
            {
                Data.accelaration.y -= entity.body.gravity - 1;
                Data.accelaration.y = Data.accelaration.y > Fix._0 ? Data.accelaration.y : Fix._0;
            }

        }
        else
        {
            Data.accelaration.y = Fix._0;
        }
    }

    private void Accelaration()
    {
        if (entity.InputPressed(EInputTypes.right))
            Data.accelaration.x += FixedMath.Fix._0_10;
        else if (entity.InputPressed(EInputTypes.left))
            Data.accelaration.x += FixedMath.Fix._0_10 * FixedMath.Fix.minus_one;
        else
        {
            if (Data.accelaration.x > Fix._0)
            {
                if (Data.accelaration.x < Fix._0_10)
                    Data.accelaration.x = Fix._0;
                else
                    Data.accelaration.x -= Fix._0_10;
            }
            if (Data.accelaration.x < Fix._0)
                if (Data.accelaration.x > Fix._0_10 * FixedMath.Fix.minus_one)
                    Data.accelaration.x = Fix._0;
                else
                    Data.accelaration.x += Fix._0_10;
        }
        var topSpeed = Fix._0_10 + Fix._0_05;
        if (entity.InputPressed(EInputTypes.run))
        {
            topSpeed = Fix._0_20 + Fix._0_10;
        }
        if (Data.accelaration.x > topSpeed)
        {
            Data.accelaration.x = topSpeed;
        }
        else if (Data.accelaration.x < Fix._0_25 * Fix.minus_one)
        {
            Data.accelaration.x = topSpeed * Fix.minus_one;
        }
        AnimationComponent animComp = null;
        entity.GetComponent(ref animComp);

        if (Data.jumping)
        {
            animComp.PlayAnimation(character.idle,true,true) ;
        }
        if (Data.accelaration.x > Fix._0_05)
        {
            Data.lookingLeft = false;
            if (!Data.jumping)
                animComp.PlayAnimation(character.run, true, true);
        }
        else if (Data.accelaration.x < Fix.minus_one * Fix._0_05)
        {
            Data.lookingLeft = true;
            if (!Data.jumping)
                animComp.PlayAnimation(character.run, true, true);
        }
        else
        {
            animComp.PlayAnimation(character.idle, true, true);
        }
    }

    private void Physics()
    {
        entity.body.gravity += Fix._0_02;
        entity.body.force.y -= entity.body.gravity < Fix._1_50 ? entity.body.gravity : Fix._1_50;
    }
}
