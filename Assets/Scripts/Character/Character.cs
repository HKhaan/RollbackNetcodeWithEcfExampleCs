
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Character", menuName = "Character/Create", order = 1)]
public class Character : ScriptableObject
{
    public GameObject mesh;
    public Avatar avatar;
    public Move idle;
    public Move run;
    public List<Move> moves;
    public List<Move> flattenedMoves;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class Move
{
    public int index;
    public AnimationClip animAsset;
    public int startFrame;
    public int endFrame;
    public HumanBodyBones hitLimb;
    public float hitboxSide;
    public EInputTypes input;
    public List<Hitboxes> hitboxPositions = new List<Hitboxes>();
}

[Serializable]
public struct Hitboxes
{
    public Hitbox[] hurtboxes;
    public Hitbox[] hitboxes;
}

[Serializable]
public struct Hitbox
{
    public Vector3 position;
    public float radius;
    public HumanBodyBones humanBodyBones;

}