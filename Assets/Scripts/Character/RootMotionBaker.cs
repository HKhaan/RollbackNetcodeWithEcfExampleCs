using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR

public class RootMotionBaker : MonoBehaviour
{
    // public FighterObjectEditor fighterObjectEditor;
    public Character character;
    public bool bakeDone = false;
    GameObject baker;

    public void Start()
    {
        Debug.Log("awake baker");
        character.flattenedMoves = new List<Move>();
        character.idle.index = character.flattenedMoves.Count;
        character.flattenedMoves.Add(character.idle);
        character.run.index = character.flattenedMoves.Count;
        character.flattenedMoves.Add(character.run);
        foreach (var move in character.moves)
        {
            move.index = character.flattenedMoves.Count;
            character.flattenedMoves.Add(move);
            
        }
        StartCoroutine(BakeAnim());

    }


    private IEnumerator BakeAnim()
    {

        while (character is null)
        {
            Debug.Log("fighterObject");
            yield return new WaitForFixedUpdate();
        }

        var animObject = Instantiate(character.mesh);
        baker = animObject;
        animObject.name = "baker";
        var pos = new Vector3(-999, -999, -999);
        animObject.transform.position = pos;
        animObject.transform.LookAt(pos + Vector3.right * 5);
        var myanimator = animObject.GetComponent<Animator>();
        if (myanimator == null)
        {
            myanimator = animObject.AddComponent<Animator>();
        }
        myanimator.avatar = character.avatar;
        myanimator.applyRootMotion = true;
        var runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Animator");
        myanimator.runtimeAnimatorController = runtimeAnimatorController;
        myanimator.enabled = false;

        foreach (var moves in character.flattenedMoves)
        {

            var anim = moves.animAsset;
            if (anim == null)
            {
                continue;
            }
            //var bones = new Dictionary<string, List<Keyframe>>();
            //try
            //{
            //    if (anim.name == "Kick12")
            //    {
            //        Debug.Log("debuig");
            //    }

            //    var curveBindings = UnityEditor.AnimationUtility.GetCurveBindings(anim);
            //    foreach (var curveBinding in curveBindings)
            //    {
            //        if (curveBinding.propertyName.Length > 2)
            //        {
            //            var axis = curveBinding.propertyName.Substring(curveBinding.propertyName.Length - 1, 1);
            //            var bone = curveBinding.propertyName.Substring(0, curveBinding.propertyName.Length - 2);
            //            if (!bones.ContainsKey(bone))
            //            {
            //                bones.Add(bone, new List<Keyframe>());
            //            }
            //            var t = UnityEditor.AnimationUtility.GetEditorCurve(anim, curveBinding);
            //            var frame = 0;
            //            foreach (var key in t.keys)
            //            {
            //                if (bones[bone].Count < frame + 1)
            //                {
            //                    bones[bone].Add(new Keyframe()); ;
            //                }
            //                if (axis == "x")
            //                    bones[bone][frame].x = key.value;
            //                if (axis == "y")
            //                    bones[bone][frame].y = key.value;
            //                if (axis == "z")
            //                    bones[bone][frame].z = key.value;
            //                if (axis == "w")
            //                    bones[bone][frame].w = key.value;
            //                frame++;
            //            }
            //            //Debug.Log("changedAnimation");
            //        }
            //        //Debug.Log("changedAnimation");
            //    }
            //}
            //catch (Exception)
            //{
            //    Debug.LogError("whjy");
            //}
            //if (anim is null)
            //{
            //    continue;
            //}
            var animatorOverrideController = new AnimatorOverrideController(myanimator.runtimeAnimatorController);
            animatorOverrideController["Anim1"] = anim;
            myanimator.applyRootMotion = true;
            myanimator.runtimeAnimatorController = animatorOverrideController;
            myanimator.Play("Anim1", 0, 0);
            float time = 0;
            var force = Vector3.zero;
            var previousForce = Vector3.zero;
            myanimator.Update(0.00001f);
            myanimator.enabled = false;
            yield return new WaitForEndOfFrame();
            List<Vector3> roots = new List<Vector3>();
            List<Quaternion> rootRotations = new List<Quaternion>();
            var hurtboxes = new List<Hitbox[]>();
            var hitboxes = new List<Hitbox[]>();
            if ((anim != null))
            {
                bool canBake = false;
                float length = 0;
                //For some reason anim != null doesn't work always
                try
                {
                    length = anim.length;
                    canBake = true;
                }
                catch (Exception ex)
                {
                    Debug.Log("No anim for move");
                }
                //if (moves.hasRootMotion)
                //{
                animObject.transform.position = pos;
                animObject.transform.LookAt(pos + Vector3.right * 5);
                while (time < length)
                {
                    var frame = (int)(Mathf.Round(time / 0.016f));
                    animObject.transform.LookAt(animObject.transform.position + Vector3.right * 5);
                    var vec = myanimator.transform; //myanimator.GetBoneTransform(HumanBodyBones.Hips);
                                                    //var vec = myanimator.GetBoneTransform(HumanBodyBones.Hips);
                    if (time == 0)
                        previousForce = vec.position;
                    var saveForce = (vec.position - previousForce);// *60;
                    previousForce = vec.position;
                    //roots.Add(saveForce);
                    //if (moves.hasRootMotion)
                    //{
                    //    if (bones != null && bones.ContainsKey("RootT"))
                    //    {
                    //        //todo:fix framerate issue
                    //        if (bones["RootT"].Count > frame)
                    //        {
                    //            if (frame == 0)
                    //            {
                    //                roots.Add(Vector3.zero);
                    //                rootRotations.Add(new Quaternion(0, 0, 0, 0));
                    //            }
                    //            else
                    //            {
                    //                var val2 = (bones["RootT"][frame].x - bones["RootT"][frame - 1].x) * -1;
                    //                var val = bones["RootT"][frame].z - bones["RootT"][frame - 1].z;
                    //                val *= 60;
                    //                val2 *= 60;
                    //                if (anim.frameRate <= 35)
                    //                {
                    //                    if ((frame % 2) == 0)
                    //                    {
                    //                        roots.Add(Vector3.zero);
                    //                        rootRotations.Add(new Quaternion(0, 0, 0, 0));
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    roots.Add(new Vector3(val2, 0, val));
                    //                    var quat = bones["RootQ"][frame];
                    //                    rootRotations.Add(new Quaternion(quat.x, quat.y, quat.z, quat.w));
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    frame++;
                    var hurtboxPositionsForFrame = new List<Hitbox>();
                    var hitboxPositionsForFrame = new List<Hitbox>();
                    if(moves.endFrame != 0)
                    {
                        Debug.Log("test");
                    }
                    if (moves.startFrame <= frame && moves.endFrame >= frame && moves.endFrame!=0)
                    {
                        var t = myanimator.GetBoneTransform(moves.hitLimb);
                        hitboxPositionsForFrame.Add(new Hitbox
                        {
                            radius = moves.hitboxSide,
                            position = t.transform.position - vec.position,
                            humanBodyBones = moves.hitLimb,
                        });
                    }
                    hitboxes.Add(hitboxPositionsForFrame.ToArray());
                    foreach (HumanBodyBones val in Enum.GetValues(typeof(HumanBodyBones)))
                    {
                        // physicMaterial = fighter.defaults?.physicMaterial;
                        var hitboxSize = (int)val >= 18 ? 0.3f : 0.2f;
                        bool isTrigger = val == HumanBodyBones.LeftHand ||
                                            val == HumanBodyBones.RightHand;
                        if ((int)val <= 18)
                        {
                            try
                            {
                                var t = myanimator.GetBoneTransform(val);
                                hurtboxPositionsForFrame.Add(new Hitbox
                                {
                                    radius = hitboxSize,
                                    position = t.transform.position - vec.position,
                                    humanBodyBones = val,
                                });

                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning(ex.Message);
                            }

                        }
                    }
                    hurtboxes.Add(hurtboxPositionsForFrame.ToArray());
                    time += 0.016f;

                    myanimator.Update(0.016f);
                    yield return new WaitForEndOfFrame();
                }
                //}
                //else
                //{
                //    moves.rootMotionData = new List<Vector3>();
                //}


                moves.hitboxPositions = new List<Hitboxes>();
                for (int i = 0; i < hurtboxes.Count; i++)
                {
                    try
                    {
                        moves.hitboxPositions.Add(new Hitboxes
                        {
                            hitboxes = hitboxes[i],
                            hurtboxes = hurtboxes[i]
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                    }
                    
                }
            }

            bakeDone = true;
            RootMotionBakerSingleton.GetInstance.RootMotionBaker = this;
        }
    }

    // Callback(new List<Vector3>(),this.gameObject);
    // DestroyImmediate(this);

    public void DestroyMe()
    {
        Destroy(baker.transform);
        Destroy(this);
    }
}

public class RootMotionBakerSingleton
{
    public RootMotionBaker RootMotionBaker;

    private static RootMotionBakerSingleton _instance = null;

    public static RootMotionBakerSingleton GetInstance
    {
        get
        {
            if (_instance is null)
            {
                _instance = new RootMotionBakerSingleton();
            }

            return _instance;
        }
    }

    private RootMotionBakerSingleton()
    {
    }
}
#endif