using FixedMath;
using RollBackExample;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    private bool syncTestEnabled = false;
    public Dictionary<int, List<object>> stateBackup = new Dictionary<int, List<object>>();
#endif
    private int[] inputs = new int[2] { 0, 0 };
    private RollbackLogic rollbackLogic;

    private List<Connections> connections = new List<Connections> {
        new Connections{port=11010, ip="127.0.0.1"},
        new Connections{port=11015, ip="127.0.0.1"}
    };
    public List<GameObject> visualizer;
    int lastRenderedFrame = -10;
    private MovementComponent movCompForDesync;
    public int PlayerIndex { get; set; }
    // Start is called before the first frame update
    //void Start()
    //{
    //    StartGame();

    //}

    private void StartGame()
    {
        world = new RollbackWorld();
        Game.amountOfPlayers = connections.Count;
        for (int i = 0; i < Game.amountOfPlayers; i++)
        {
            CreatePlayer(i);
            if (i != 0)
            {
                visualizer.Add(Instantiate(visualizer[0].gameObject));
            }
        }
        int index = 0;
        foreach (var vis in visualizer)
        {
            vis.GetComponent<VisualizerAnimator>().StartEntity(world.entities[index]);
            index++;
        }
        CubeConverter[] components = GameObject.FindObjectsOfType<CubeConverter>();
        foreach (var comp in components)
        {
            comp.Convert(world);
        }

        rollbackLogic = new RollbackLogic(world);
        Game.RunGameLogic = (ulong[] inputs) =>
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
        Game.RunGameLogic(new ulong[] { 0 });
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
        var ngs = new NonGameState();
        Game.InitGame(ngs);
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

            var players = new List<GGPOPlayer>();
            for (int i = 0; i < connections.Count; ++i)
            {
                var player = new GGPOPlayer();
                if (PlayerIndex == i)
                {
                    player.type = GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL;
                    player.ip_address = connections[i].ip;
                    player.port = connections[i].port;
                }
                else
                {
                    player.type = GGPOPlayerType.GGPO_PLAYERTYPE_REMOTE;
                    player.ip_address = connections[i].ip;
                    player.port = connections[i].port;
                }
                player.player_num = i + 1;
                players.Add(player);
            }
            ngs.players = players.Select(x =>
            {
                return new PlayerConnectionInfo
                {
                    type = x.type,
                    controllerId = x.player_num
                };
            }).ToArray();
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
        movCompForDesync.character = visualizer[0].GetComponent<VisualizerEntity>().character;
        var animComp= new AnimationComponent();
        var hitComponent = new HitComponent();
        var hurtComponent = new HurtComponent();
        ent.components.Add(movCompForDesync);
        ent.components.Add(hitComponent);
        ent.components.Add(animComp);
        ent.components.Add(hurtComponent);
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
        if (!started) return;
        Game.Idle(2);
        Game.RunFrame();

        //TODO: do this in a better way
        for (int i = 0; i < connections.Count; i++)
        {
            var t = (world.entities[i].body as Rectangle).dimensions.ToUnityVec() / 2.0f;
            t.z = 0;
            var meshOffset = Vector3.up * 0.5f;
            visualizer[i].transform.position = world.entities[i].body.position.ToUnityVec() - t - meshOffset;
        }
    }

    private bool started = false;
    private void OnGUI()
    {
        if (started) return;
        for (int i = 0; i < connections.Count; i++)
        {
            if (GUI.Button(new Rect(new Vector2(0, (i + 1) * 20), new Vector2(200, 20)), "Play as this player"))
            {
                started = true;
                PlayerIndex = i;
                StartGame();
            }
            connections[i].port = ushort.Parse(GUI.TextField(new Rect(new Vector2(200, (i + 1) * 20), new Vector2(100, 20)), connections[i].port.ToString()));
        }
#if UNITY_EDITOR
        if (GUI.Button(new Rect(new Vector2(0, 0), new Vector2(200, 20)), "Sync test"))
        {
            started = true;
            PlayerIndex = 0;
            syncTestEnabled = true;
            connections = new List<Connections> {
                new Connections{port=11010, ip="127.0.0.1"},
                new Connections{port=11010, ip="127.0.0.1"},
            };
            StartGame();
        }
#endif
    }
}
