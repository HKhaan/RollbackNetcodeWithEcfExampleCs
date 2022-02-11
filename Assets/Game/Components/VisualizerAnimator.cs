using RollBackExample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizerAnimator : MonoBehaviour
{
    private Entity entity;
    public float animatorTime = 0.0f;
    public float delta = 0.0f;
    // Start is called before the first frame update
    private Animator rootMesh;
    public AnimationClip currentAnimation;
    public Move move;
    private Animator animator;
    AnimatorOverrideController animatorOverrideController;
    AnimationComponent animationComponent = null;
    MovementComponent movementComponent = null;
    void Start()
    {
        animator = GetComponent<Animator>();
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.enabled = false;
    }

    public void PlayAnimation(AnimationClip animationClip)
    {
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animatorOverrideController["Anim1"] = animationClip;
        animator.runtimeAnimatorController = animatorOverrideController;
        animator.Play("Anim1", 0, 0);
        currentAnimation = animationClip;
        animatorTime = 0.0f;

    }
    public void StartEntity(Entity entity)
    {
        this.entity = entity;

        entity.GetComponent(ref animationComponent);
        entity.GetComponent(ref movementComponent);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAnimation != animationComponent?.move?.animAsset)
        {
            PlayAnimation(animationComponent.move.animAsset);
        }
        if (currentAnimation != null && animationComponent.move != null)
        {
            move = animationComponent.move;
            var newTime = (float)animationComponent.Data.CurrentTime;
            delta = newTime - animatorTime;
            animatorTime = newTime;
            animator.Update(delta);
            transform.localScale = new Vector3(movementComponent.Data.lookingLeft ? -1 : 1, 1, 1);


        }
    }

    private void OnDrawGizmos()
    {
        if (move != null && move.hitboxPositions.Count>0)
        {
            var hboxes = move.hitboxPositions[(int)(animatorTime * 60.0f)].hurtboxes;
            Gizmos.color = Color.green;
            foreach (var hbox in hboxes)
            {
                var hboxpos = hbox.position;
                if (movementComponent.Data.lookingLeft)
                {
                    hboxpos.x *= -1.0f;
                }
                Gizmos.DrawSphere(this.transform.position + hboxpos, 0.1f);

            }
            hboxes = move.hitboxPositions[(int)(animatorTime * 60.0f)].hitboxes;
            Gizmos.color = Color.red;
            foreach (var hbox in hboxes)
            {
                var hboxpos = hbox.position;
                if (movementComponent.Data.lookingLeft)
                {
                    hboxpos.x *= -1.0f;
                }
                Gizmos.DrawSphere(this.transform.position + hboxpos, 0.1f);

            }
        }
    }
}
