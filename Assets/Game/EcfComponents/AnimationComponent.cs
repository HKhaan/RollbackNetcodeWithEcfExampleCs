using FixedMath;
using RollBackExample;
using System;
[Serializable]
public struct AnimationData
{
    public Fix CurrentTime;
    public bool ResetOnEnd;
    public bool Ended ;
    public int moveIndex;
    //this is for a test (should remove this before pushing)
}
public class AnimationComponent : Component
{
    Entity entity;
    [RollBackProp]
    public AnimationData Data;
    public Move move;
    public MovementComponent movementComponent;
    public AnimationComponent()
    {
    }

    public int OwnerId { get; set; }

    public int ExecuteIndex => 1;
    public void Start(Entity owner)
    {
        entity = owner;
        owner.GetComponent(ref movementComponent);
    }
    public void Update()
    {
        GetMove();
        if (move != null)
        {
            if (new Fix(move.animAsset.length) < Data.CurrentTime)
            {
                if (Data.ResetOnEnd)
                {
                    Data.CurrentTime -= new Fix(move.animAsset.length);

                }
                else
                {
                    Data.Ended = true;
                }
            }
            else
            {
                Data.CurrentTime += Fix._0_016;
            }
        }
    }

    public Move GetMove()
    {
        if (Data.moveIndex == -1)
        {
            move = null;
        }
        else if (move == null || move.index != Data.moveIndex)
        {
            move = movementComponent.character.flattenedMoves.Find(x => x.index == Data.moveIndex); //TODO find better way to do this. Linq is not super fast.
        }
        return move;
    }

    public void PlayAnimation(Move move,bool dontReplayIfSame,bool resetOnEnd)
    {
        if (dontReplayIfSame && Data.moveIndex== move.index) return;
        this.move = move;
        Data.moveIndex = move.index;
        Data.CurrentTime = Fix._0;
        Data.ResetOnEnd = resetOnEnd;
        Data.Ended = false;
    }

}
