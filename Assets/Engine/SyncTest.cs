using FixedMath;
using RollBackExample;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[System.Serializable]
public class Connections
{
    public ushort port;
    public string ip;
    public bool spectator;
}

public class SyncTest : MonoBehaviour
{
    private RollbackWorld world;

#if UNITY_EDITOR
    public bool syncTestEnabled = false;
    public Dictionary<int, List<object>> stateBackup = new Dictionary<int, List<object>>();
#endif
    public int[] inputs = new int[2] { 0, 0 };
    private RollbackLogic rollbackLogic;

    public int editorPlayerIndex = 0;
    public int otherPlayerIndex = 1;
    public List<Connections> connections;
    public GameObject visualizer;
    int lastRenderedFrame = -10;
    private MovementComponent movCompForDesync;

    public int PlayerIndex
    {
        get => Application.isEditor ? editorPlayerIndex : otherPlayerIndex;
        set
        {
            if (Application.isEditor)
            {
                editorPlayerIndex = value;
            }
            else
            {
                otherPlayerIndex = value;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        world = new RollbackWorld();
        for (int i = 0; i < connections.Count; i++)
        {
            CreatePlayer(i);
        }
        CubeConverter[] components = GameObject.FindObjectsOfType<CubeConverter>();
        foreach (var comp in components)
        {
            comp.Convert(world);
        }

        rollbackLogic = new RollbackLogic(world);
        Game.RunFrameCallback = (ulong[] inputs) =>
        {
#if UNITY_EDITOR
            if (syncTestEnabled)
            {
                BackupFieldsForDesyncComparison();
                CauseDesync();
            }
#endif
            world.Update(inputs);
#if UNITY_EDITOR
            if (syncTestEnabled)
            {
                rollbackLogic.BackupValues(Game.gameState.CurrentFrame);
                CheckDesync();
            }
#endif
        };
        Game.RunFrameCallback(new ulong[] { 0 });
        rollbackLogic.CacheFields();

        Game.rollbackLogic = rollbackLogic;
        StartGgpo();

    }

#if UNITY_EDITOR
    private void CauseDesync()
    {
        if (Input.GetKey(KeyCode.Backspace))
        {
            movCompForDesync.Data.jumping = !movCompForDesync.Data.jumping;
        }
    }
    private void BackupFieldsForDesyncComparison()
    {

        //if (Game.gameState.CurrentFrame == lastRenderedFrame - 1)
        {
            var backFields = new List<object>();
            foreach (var rollbackfield in rollbackLogic.rollbackFields)
            {
                foreach (var value in rollbackfield.Values[lastRenderedFrame])
                {
                    backFields.Add(value.Key.GetValue(rollbackfield.Component));
                }
            }
            if (stateBackup.ContainsKey(Game.gameState.CurrentFrame))
            {
                stateBackup.Remove(Game.gameState.CurrentFrame);
            }
            stateBackup.Add(Game.gameState.CurrentFrame, backFields);
        }
    }
    private void CheckDesync()
    {
        if (Game.gameState.CurrentFrame <= lastRenderedFrame)
        {
            int index = 0;
            try
            {


                var backFields = stateBackup[Game.gameState.CurrentFrame];
                foreach (var rollbackfield in rollbackLogic.rollbackFields)
                {
                    try
                    {


                        foreach (var value in rollbackfield.Values[index])
                        {
                            var curValue = value.Key.GetValue(rollbackfield.Component);
                            var prevVal = backFields[index];
                            if (!prevVal.Equals(curValue))
                            {
                                UnityEngine.Debug.LogError($"desyncVal:{curValue.GetType().Name}");
                                Debugger.Break();
                            }
                            index++;

                        }
                    }
                    catch (System.Exception)
                    {
                        //don't compare if doesn't exist.
                    }
                }
            }
            catch (System.Exception)
            {
                //don't compare if state wasnt saved
            }
            rollbackLogic.SetBack(Game.gameState.CurrentFrame);

        }
        if (Game.gameState.CurrentFrame > lastRenderedFrame)
        {
            lastRenderedFrame = Game.gameState.CurrentFrame;
        }
    }
#endif

    private void StartGgpo()
    {
        var remote_index = -1;
        var num_players = 0;
        for (int i = 0; i < connections.Count; ++i)
        {
            if (i != PlayerIndex && remote_index == -1)
            {
                remote_index = i;
            }

            if (!connections[i].spectator)
            {
                ++num_players;
            }
        }
        if (!connections[PlayerIndex].spectator)
        {
            Game.InitGame(new NonGameState());
            var players = new List<GGPOPlayer>();
            for (int i = 0; i < connections.Count; ++i)
            {
                var player = new GGPOPlayer
                {
                    player_num = players.Count + 1,
                };
                if (PlayerIndex == i)
                {
                    player.type = GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL;
                    player.ip_address = "";
                    player.port = 0;
                }
                else if (connections[i].spectator)
                {
                    player.type = GGPOPlayerType.GGPO_PLAYERTYPE_SPECTATOR;
                    player.ip_address = connections[remote_index].ip;
                    player.port = connections[remote_index].port;
                }
                else
                {
                    player.type = GGPOPlayerType.GGPO_PLAYERTYPE_REMOTE;
                    player.ip_address = connections[remote_index].ip;
                    player.port = connections[remote_index].port;
                }
                players.Add(player);
            }
#if UNITY_EDITOR
            Game.StartGame(connections[PlayerIndex].port, num_players, players, 0, syncTestEnabled);
#else
            Game.StartGame(connections[PlayerIndex].port, num_players, players, 0, false);
#endif
        }
    }

    private void CreatePlayer(int inputIndex)
    {
        var ent = new Entity();
        ent.body = new Rectangle { Owner = ent, dimensions = new FixVector(Fix._0_50, Fix._0_50, Fix._0_50), enabled = true };
        ent.body.position.x = Fix._5 + (Fix._5 * inputIndex);
        movCompForDesync = new MovementComponent();
        movCompForDesync.Data.lastCheckpoint = ent.body.position;
        ent.components.Add(movCompForDesync);
        if (inputIndex == 0)
        {
            Game.gameState = new GameStateComponent();
            ent.components.Add(Game.gameState);
        }
        ent.inputIndex = inputIndex;
        ent.receivesInput = true;
        world.AddEntity(ent);
    }
    // Update is called once per frame


    void FixedUpdate()
    {
        Game.RunFrame();
        Game.Idle(1);

        //TODO: do this in a better way
        var t = (world.entities[0].body as Rectangle).dimensions.ToUnityVec() / 2.0f;
        t.z = 0;
        visualizer.transform.position = world.entities[0].body.position.ToUnityVec() - t;
    }


}
