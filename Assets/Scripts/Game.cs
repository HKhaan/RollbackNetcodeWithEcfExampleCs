using RollBackExample;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Collections;


public static class Game
{
    const int FRAME_DELAY = 2;

    //public static GGPOPerformance perf = null;

    public static event Action<string> OnLog;
    public static NonGameState ngs;
    public static int amountOfPlayers;
    public static Stopwatch frameWatch = new Stopwatch();
    public static Stopwatch idleWatch = new Stopwatch();
    public static Action<ulong[]> RunGameLogic;
    public static RollbackLogic rollbackLogic;
    public static GameStateComponent gameState;

    /*
* game_begin_game_callback --
*
* The begin game callback.  We don't need to do anything special here,
* so just return true.
*/

    static bool Game_begin_game_callback(string name)
    {
        OnLog?.Invoke($"game_begin_game_callback");
        return true;
    }

    /*
     * game_on_event_callback --
     *
     * Notification from GGPO that something has happened.  Update the status
     * text at the bottom of the screen to notify the user.
     */

    static bool OnEventConnectedToPeerDelegate(int connected_player)
    {
        ngs.SetConnectState(connected_player, PlayerConnectState.Synchronizing);
        return true;
    }

    static public bool OnEventSynchronizingWithPeerDelegate(int synchronizing_player, int synchronizing_count, int synchronizing_total)
    {
        var progress = 100 * synchronizing_count / synchronizing_total;
        ngs.UpdateConnectProgress(synchronizing_player, progress);
        return true;
    }

    static public bool OnEventSynchronizedWithPeerDelegate(int synchronized_player)
    {
        ngs.UpdateConnectProgress(synchronized_player, 100);
        return true;
    }

    static public bool OnEventRunningDelegate()
    {
        ngs.SetConnectState(PlayerConnectState.Running);
        SetStatusText("");
        return true;
    }

    static public bool OnEventConnectionInterruptedDelegate(int connection_interrupted_player, int connection_interrupted_disconnect_timeout)
    {
        ngs.SetDisconnectTimeout(connection_interrupted_player,
                                 Helper.TimeGetTime(),
                                 connection_interrupted_disconnect_timeout);
        return true;
    }


    static public bool OnEventConnectionResumedDelegate(int connection_resumed_player)
    {
        ngs.SetConnectState(connection_resumed_player, PlayerConnectState.Running);
        return true;
    }


    static public bool OnEventDisconnectedFromPeerDelegate(int disconnected_player)
    {
        ngs.SetConnectState(disconnected_player, PlayerConnectState.Disconnected);
        return true;
    }

    public static int framesAhead;
    static public bool OnEventEventcodeTimesyncDelegate(int timesync_frames_ahead)
    {
        framesAhead = timesync_frames_ahead;
        return true;
    }


    /*
     * game_advance_frame_callback --
     *
     * Notification from GGPO we should step foward exactly 1 frame
     * during a rollback.
     */

    static bool Game_advance_frame_callback(int flags)
    {
        OnLog?.Invoke($"game_begin_game_callback {flags}");

        // Make sure we fetch new inputs from GGPO and use those to update the game state
        // instead of reading from the keyboard.
        ulong[] inputs = new ulong[amountOfPlayers];
        ReportFailure(GGPO.Session.SynchronizeInput(inputs, amountOfPlayers, out var disconnect_flags));

        AdvanceFrame(inputs, disconnect_flags);
        return true;
    }

    /*
     * game_load_game_state_callback --
     *
     * Makes our current state match the state passed in by GGPO.
     */

    static bool Game_load_game_state_callback(NativeArray<byte> data)
    {
        OnLog?.Invoke($"game_load_game_state_callback {data.Length}");
        byte[] bytes = data.ToArray();
        int frame = BitConverter.ToInt32(bytes, 0);
        rollbackLogic.SetBack(frame);
        return true;
    }

    /*
     * game_save_game_state_callback --
     *
     * Save the current state to a buffer and return it to GGPO via the
     * buffer and len parameters.
     */

    static bool Game_save_game_state_callback(out NativeArray<byte> data, out int checksum, int frame)
    {
        OnLog?.Invoke($"game_save_game_state_callback {frame}");
        byte[] bytes = BitConverter.GetBytes(gameState.CurrentFrame);
        data = new NativeArray<byte>(bytes, Allocator.Persistent);
        rollbackLogic.BackupValues(gameState.CurrentFrame);
        checksum = 0;
        return true;
    }

    /*
     * game_log_game_state --
     *
     * Log the gamestate.  Used by the synctest debugging tool.
     */

    static bool Game_log_game_state(string filename, NativeArray<byte> data)
    {
        //Todo: Add logs
        return true;
    }

    static void Game_free_buffer_callback(NativeArray<byte> data)
    {
        OnLog?.Invoke($"game_free_buffer_callback");
        if (data.IsCreated)
        {
            data.Dispose();
        }
    }

    /*
     * Init --
     *
     * Initialize the vector war game.  This initializes the game state and
     * the video renderer and creates a new network session.
     */

    public static void StartGame(int localport, int num_players, IList<GGPOPlayer> players, int num_spectators, bool syncTest)
    {
        // Initialize the game state
        amountOfPlayers = num_players;
        int result=0;
        if (syncTest)
        {
            result = GGPO.Session.StartSyncTestSession(Game_begin_game_callback,
            Game_advance_frame_callback,
            Game_load_game_state_callback,
            Game_log_game_state,
            Game_save_game_state_callback,
            Game_free_buffer_callback,
            OnEventConnectedToPeerDelegate,
            OnEventSynchronizingWithPeerDelegate,
            OnEventSynchronizedWithPeerDelegate,
            OnEventRunningDelegate,
            OnEventConnectionInterruptedDelegate,
            OnEventConnectionResumedDelegate,
            OnEventDisconnectedFromPeerDelegate,
            OnEventEventcodeTimesyncDelegate,
            "game", num_players, 4);
        }
        else
        {
            result = GGPO.Session.StartSession(Game_begin_game_callback,
                    Game_advance_frame_callback,
                    Game_load_game_state_callback,
                    Game_log_game_state,
                    Game_save_game_state_callback,
                    Game_free_buffer_callback,
                    OnEventConnectedToPeerDelegate,
                    OnEventSynchronizingWithPeerDelegate,
                    OnEventSynchronizedWithPeerDelegate,
                    OnEventRunningDelegate,
                    OnEventConnectionInterruptedDelegate,
                    OnEventConnectionResumedDelegate,
                    OnEventDisconnectedFromPeerDelegate,
                    OnEventEventcodeTimesyncDelegate,
                    "game", num_players, localport);

        }
        ReportFailure(result);

        // automatically disconnect clients after 3000 ms and start our count-down timer for
        // disconnects after 1000 ms. To completely disable disconnects, simply use a value of 0
        // for ggpo_set_disconnect_timeout.
        ReportFailure(GGPO.Session.SetDisconnectTimeout(60000));
        ReportFailure(GGPO.Session.SetDisconnectNotifyStart(60000));

        int controllerId = 0;
        int playerIndex = 0;
        ngs.players = new PlayerConnectionInfo[num_players];
        for (int i = 0; i < players.Count; i++)
        {
            ReportFailure(GGPO.Session.AddPlayer(players[i], out int handle));

            if (players[i].type == GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL)
            {
                var playerInfo = new PlayerConnectionInfo();
                playerInfo.handle = handle;
                playerInfo.type = players[i].type;
                playerInfo.connect_progress = 100;
                playerInfo.controllerId = controllerId++;
                ngs.players[playerIndex++] = playerInfo;
                ngs.SetConnectState(handle, PlayerConnectState.Connecting);
                //ReportFailure(GGPO.Session.SetFrameDelay(handle, FRAME_DELAY));
            }
            else if (players[i].type == GGPOPlayerType.GGPO_PLAYERTYPE_REMOTE)
            {
                var playerInfo = new PlayerConnectionInfo();
                playerInfo.handle = handle;
                playerInfo.type = players[i].type;
                playerInfo.connect_progress = 0;
                ngs.players[playerIndex++] = playerInfo;
            }
        }

        //perf.ggpoutil_perfmon_init();
        SetStatusText("Connecting to peers.");
    }

    /*
     * InitSpectator --
     *
     * Create a new spectator session
     */

    public static void InitSpectator(int localport, int num_players, string host_ip, int host_port)
    {
        // Initialize the game state
        ngs.players = Array.Empty<PlayerConnectionInfo>();

        // Fill in a ggpo callbacks structure to pass to start_session.
        var result = GGPO.Session.StartSpectating(
                Game_begin_game_callback,
                Game_advance_frame_callback,
                Game_load_game_state_callback,
                Game_log_game_state,
                Game_save_game_state_callback,
                Game_free_buffer_callback,
                OnEventConnectedToPeerDelegate,
                OnEventSynchronizingWithPeerDelegate,
                OnEventSynchronizedWithPeerDelegate,
                OnEventRunningDelegate,
                OnEventConnectionInterruptedDelegate,
                OnEventConnectionResumedDelegate,
                OnEventDisconnectedFromPeerDelegate,
                OnEventEventcodeTimesyncDelegate,
                "game", num_players, localport, host_ip, host_port);

        ReportFailure(result);

        //perf.ggpoutil_perfmon_init();

        SetStatusText("Starting new spectator session");
    }

    /*
     * DisconnectPlayer --
     *
     * Disconnects a player from this session.
     */

    public static void DisconnectPlayer(int playerIndex)
    {
        if (playerIndex < ngs.players.Length)
        {
            string logbuf;
            var result = GGPO.Session.DisconnectPlayer(ngs.players[playerIndex].handle);
            if (GGPO.SUCCEEDED(result))
            {
                logbuf = $"Disconnected player {playerIndex}.";
            }
            else
            {
                logbuf = $"Error while disconnecting player (err:{result}).";
            }
            SetStatusText(logbuf);
        }
    }

    /*
     * AdvanceFrame --
     *
     * Advances the game state by exactly 1 frame using the inputs specified
     * for player 1 and player 2.
     */
    static void AdvanceFrame(ulong[] inputs, int disconnect_flags)
    {
        RunGameLogic(inputs);
        GGPO.Session.AdvanceFrame();
    }

    /*
     * ReadInputs --
     *
     * Read the inputs for player 1 from the keyboard.  We never have to
     * worry about player 2.  GGPO will handle remapping his inputs
     * transparently.
     */

    public static ulong ReadInputs(int id)
    {
        ulong input = 0;

        if (id == 0)
        {
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.D))
            {
                input |= (ulong)EInputTypes.right;
            }
            //Support for both AZERTY & QWERTY
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Q) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.A))
            {
                input |= (ulong)EInputTypes.left;
            }

            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Z) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.W))
            {
                input |= (ulong)EInputTypes.up;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.S))
            {
                input |= (ulong)EInputTypes.down;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Space))
            {
                input |= (ulong)EInputTypes.jump;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftShift))
            {
                input |= (ulong)EInputTypes.run;
            }


        }
        else if (id == 1)
        {
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.UpArrow))
            {
                input |= (ulong)EInputTypes.up;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.DownArrow))
            {
                input |= (ulong)EInputTypes.down;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftArrow))
            {
                input |= (ulong)EInputTypes.left;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightArrow))
            {
                input |= (ulong)EInputTypes.right;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightControl))
            {
                input |= (ulong)EInputTypes.jump;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightShift))
            {
                input |= (ulong)EInputTypes.run;
            }
        }

        return input;
    }

    /*
     * RunFrame --
     *
     * Run a single frame of the game.
     */

    public static void RunFrame()
    {
        if (framesAhead > 0)
        {
            framesAhead--;
            return;
        }
        var result = GGPO.OK;

        for (int i = 0; i < ngs.players.Length; ++i)
        {
            var player = ngs.players[i];
            if (player.type == GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL)
            {
                ulong input = ReadInputs(player.controllerId);
                result = GGPO.Session.AddLocalInput(player.handle, input);
            }
        }
        if (result == GGPO.ERRORCODE_NOT_SYNCHRONIZED)
        {
            Game.Idle(15);
            System.Threading.Thread.Sleep(15);
        }
        // synchronize these inputs with ggpo. If we have enough input to proceed ggpo will
        // modify the input list with the correct inputs to use and return 1.
        if (GGPO.SUCCEEDED(result))
        {
            frameWatch.Start();
            ulong[] inputs = new ulong[amountOfPlayers];
            result = GGPO.Session.SynchronizeInput(inputs, amountOfPlayers, out var disconnect_flags);
            if (GGPO.SUCCEEDED(result))
            {
                // inputs[0] and inputs[1] contain the inputs for p1 and p2. Advance the game by
                // 1 frame using those inputs.
                AdvanceFrame(inputs, disconnect_flags);

            }
            else
            {
                OnLog?.Invoke("Error inputsync");
            }
            frameWatch.Stop();
        }
    }

    /*
     * Idle --
     *
     * Spend our idle time in ggpo so it can use whatever time we have left over
     * for its internal bookkeeping.
     */

    public static void Idle(int time)
    {
        idleWatch.Start();
        ReportFailure(GGPO.Session.Idle(time));
        idleWatch.Stop();
    }

    public static void Exit()
    {
        if (GGPO.Session.IsStarted())
        {
            ReportFailure(GGPO.Session.CloseSession());
        }
    }

    static void SetStatusText(string status)
    {
        ngs.status = status;
    }

    public static void InitGame(NonGameState _ngs)
    {
        ngs = _ngs;
    }

    static void ReportFailure(int result)
    {
        if (!GGPO.SUCCEEDED(result))
        {
            OnLog?.Invoke(GGPO.GetErrorCodeMessage(result));
        }
    }
}
