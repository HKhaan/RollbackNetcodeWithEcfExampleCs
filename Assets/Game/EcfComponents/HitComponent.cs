using FixedMath;
using RollBackExample;
using System;
[Serializable]
public struct HitData
{
    public bool hitting;

    //this is for a test (should remove this before pushing)
}
public class HitComponent : Component
{
    Entity entity;
    [RollBackProp]
    public HitData Data;
    private AnimationComponent animationComponent = null;
    private MovementComponent movementComponent = null;
    Move move;
    public HitComponent()
    {
    }

    public int OwnerId { get; set; }

    public int ExecuteIndex => 1;

    public void Update()
    {

        if (Data.hitting)
        {
            if (animationComponent.Data.Ended)
            {
                Data.hitting = false;
                return;
            }
            else
            {
                var frame = (int)(animationComponent.Data.CurrentTime / Fix._0_016);
                var mv = animationComponent.GetMove();
                if (mv == null) return;
                if (mv.hitboxPositions.Count > frame && mv.hitboxPositions[frame].hitboxes.Length!=0)
                {
                    var hitboxPos = FixVector.FromVector3(mv.hitboxPositions[frame].hitboxes[0].position);
                    if (movementComponent.Data.lookingLeft) { 
                        hitboxPos.x *= Fix.minus_one;
                    }
                    var worldHitboxPos = entity.body.position + hitboxPos;
                    if (frame >= mv.startFrame && frame <= mv.startFrame)
                    {
                        UnityEngine.Debug.Log("active");
                        for (int i = 0; i < Game.amountOfPlayers; i++)
                        {
                            var enemy = RollbackWorld.Instance.entities[i];
                            if (enemy == entity) continue;
                            AnimationComponent eAnimComponent = null;
                            MovementComponent eMovementComponent = null;
                            HurtComponent eHurtComponent = null;
                            enemy.GetComponent(ref eAnimComponent);
                            enemy.GetComponent(ref eHurtComponent);
                            enemy.GetComponent(ref eHurtComponent);
                            enemy.GetComponent(ref eMovementComponent);
                            var emv = eAnimComponent.GetMove();
                            if(emv.hitboxPositions.Count > frame && emv.hitboxPositions[frame].hurtboxes.Length != 0)
                            {
                                foreach (var ehurtbox in emv.hitboxPositions[frame].hurtboxes)
                                {
                                    var hurtboxPos = FixVector.FromVector3(ehurtbox.position);

                                    if (eMovementComponent.Data.lookingLeft)
                                    {
                                        hurtboxPos.x *= Fix.minus_one;
                                    }
                                    var eWorldHurtboxPos = enemy.body.position + hurtboxPos;
                                    var dist = FixVector.Distance(worldHitboxPos, eWorldHurtboxPos);
                                    if (dist < Fix._0_3)
                                    {
                                        eHurtComponent.Data.gotHurt = true;
                                        eHurtComponent.Data.isLeft = enemy.body.position.x > entity.body.position.x;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (entity.InputPressed(EInputTypes.hit))
            {
                animationComponent.PlayAnimation(movementComponent.character.moves[0], false, false);
                Data.hitting = true;
            }
        }
    }

    public void Start(Entity owner)
    {
        entity = owner;
        owner.GetComponent(ref animationComponent);
        owner.GetComponent(ref movementComponent);
    }
}
