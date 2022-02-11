using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.Rendering;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor
{
    private Character character;
    private Animator animator;
    public bool baked = false;
    [FormerlySerializedAs("bakedHeight")] public List<Vector3> bakedForce = new List<Vector3>();
    float x, y, z;
    private void OnEnable()
    {
        character = (Character)target;
    }

    public override void OnInspectorGUI()
    {

        if (RootMotionBakerSingleton.GetInstance.RootMotionBaker!=null && RootMotionBakerSingleton.GetInstance.RootMotionBaker.bakeDone)
        {
            if (GUILayout.Button("ApplyBake"))
            {
                RootMotionBakerSingleton.GetInstance.RootMotionBaker.bakeDone = false;
                var _rootMotionBaker = RootMotionBakerSingleton.GetInstance.RootMotionBaker;
                EditorApplication.ExecuteMenuItem("Edit/Play");
                character.flattenedMoves = _rootMotionBaker.character.flattenedMoves;
                _rootMotionBaker.DestroyMe();
                RootMotionBakerSingleton.GetInstance.RootMotionBaker = null;
            }
        }
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Mesh");
            character.mesh = (GameObject)EditorGUILayout.ObjectField(character.mesh, typeof(GameObject), false,
                GUILayout.Width(200f), GUILayout.Height(20f));
            GUILayout.Label("Avatar");
            character.avatar = (Avatar)EditorGUILayout.ObjectField(character.avatar, typeof(Avatar), false,
                GUILayout.Width(200f), GUILayout.Height(20f));
            GUILayout.Label("idle");
            var idle = (AnimationClip)EditorGUILayout.ObjectField(character.idle.animAsset,
                typeof(AnimationClip), false, GUILayout.Width(200f), GUILayout.Height(20f));
            character.idle.animAsset = idle;
            GUILayout.Label("Run");
            var run = (AnimationClip)EditorGUILayout.ObjectField(character.run.animAsset,
                typeof(AnimationClip), false, GUILayout.Width(200f), GUILayout.Height(20f));
            character.run.animAsset = run;
        }
        if (GUILayout.Button("AddMove"))
        {
            if (character.moves == null)
            {
                character.moves = new List<Move>();
            }
            character.moves.Add(new Move());
        }
        foreach (var move in character?.moves)
        {
            //spacer
            {
                var gs = new GUIStyle();
                gs.normal.background = MakeTex(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.0f));

                GUILayout.BeginHorizontal(gs);
                GUILayout.BeginVertical();
                GUILayout.Label("");
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            //move editor
            {
                var gs = new GUIStyle();
                gs.normal.background = MakeTex(600, 1, new Color(0.0f, 0.0f, 0.0f, 0.3f));

                GUILayout.BeginHorizontal(gs);
                GUILayout.BeginVertical();
                var animation = (AnimationClip)EditorGUILayout.ObjectField(move.animAsset,
                    typeof(AnimationClip), false, GUILayout.Width(200f), GUILayout.Height(20f));
                move.animAsset = animation;
                move.input = (EInputTypes)(EditorGUILayout.EnumFlagsField("Input", move.input));
                
                move.startFrame = EditorGUILayout.IntField("startFrame", move.startFrame);
                move.endFrame = EditorGUILayout.IntField("endFrame", move.endFrame);
                move.hitLimb = (HumanBodyBones)(EditorGUILayout.EnumFlagsField("HitLimb", move.hitLimb));
                //GUILayout.Label($"Input");

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        EditorUtility.SetDirty(character);
    }


    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}


