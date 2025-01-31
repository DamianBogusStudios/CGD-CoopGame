// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to connect, and join/create room automatically
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

#pragma warning disable 649

namespace CGD.Networking
{
    public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks, IMatchmakingCallbacks
    {

        public delegate void UpdateRoomListDelegate(List<RoomInfo> roomList);
        public event UpdateRoomListDelegate RoomListUpdated;

        public delegate void UpdateRoomPlayerList();
        public event UpdateRoomPlayerList RoomPlayersUpdated;

        #region Private Serializable Fields
        [Tooltip("The Ui Text to inform the user about the connection progress")]
        [SerializeField]
        private TextMeshProUGUI feedbackText;


        [Tooltip("The maximum number of players per room")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        #endregion

        #region Private Fields
        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool isConnecting;

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1.0";

        /// <summary>
        /// Reference to fusion NetworkRunner class, added on starting game
        /// </summary>
        NetworkRunner networkRunner;


        /// <summary>
        /// Reference to lbc used for managing romos and lobbies.
        /// </summary>
        LoadBalancingClient loadBalancingClient;
        #endregion

        #region MonoBehaviour CallBacks
        void Awake()
        {
            //PhotonNetwork.AutomaticallySyncScene = true;
            //{
            //    LogFeedback("Connecting...");

            //    // #Critical, we must first and foremost connect to Photon Online Server.
            //    isConnecting = true;
            //    PhotonNetwork.ConnectUsingSettings();
            //    PhotonNetwork.GameVersion = this.gameVersion;
            //}
        }

        void Start() 
        {
            StartHost();
            //StartGame(GameMode.AutoHostOrClient);
        }
        #endregion


        #region Public Methods
        public void StartGame()
        {
            if (!PhotonNetwork.IsConnected || !PhotonNetwork.IsMasterClient)
                return;

            //TODO check for min players
#if DEBUGGING
            Debug.Log("Loading Game Scene");
#endif

            PhotonNetwork.LoadLevel(1);
        }

        /// <summary>
        /// Logs the feedback in the UI view for the player, as opposed to inside the Unity Editor for the developer.
        /// </summary>
        /// <param name="message">Message.</param>
        void LogFeedback(string message)
        {
            //TODO animate elipses for user feedback.

            // we do not assume there is a feedbackText defined.
            if (feedbackText == null)
            {
                return;
            }

            // add new messages as a new line and at the bottom of the log.
            feedbackText.text = System.Environment.NewLine + message;
        }


        public void JoinRoom(RoomInfo roomInfo)
        {
            PhotonNetwork.JoinRoom(roomInfo.Name);
            MenuManager.Instance.OpenMenu("loading");
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            MenuManager.Instance.OpenMenu("main");

        }
        #endregion

        #region Private Methods
        private void ToMainMenu()
        {
            MenuManager.Instance.OpenMenu("main");
        }
        #endregion

        #region Connection
        private async void StartHost() 
        {
            networkRunner = gameObject.AddComponent<NetworkRunner>();
            networkRunner.ProvideInput = true;

            // Create the NetworkSceneInfo from the current scene
            var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            var sceneInfo = new NetworkSceneInfo();
            if (scene.IsValid)
            {
                sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
            }

            // Start or join (depends on gamemode) a session with a specific name
            var result = await networkRunner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.AutoHostOrClient,
                Scene = scene,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });


            if (result.Ok) 
            {
#if DEBUGGING
                Debug.Log(networkRunner.SessionInfo.Name);
#endif
            }
            else
            {
#if DEBUGGING
                Debug.LogError("error starting game: " + result.ErrorMessage);
#endif
            }
        }
        public void RequestCreateRoom()
        {

#if DEBUGGING
            Debug.Log("Creating new room: " + loadBalancingClient);
#endif

            MenuManager.Instance.OpenMenu("loading");

            RoomOptions roomOptions = new RoomOptions();
            //roomOptions.CustomRoomPropertiesForLobby = { MAP_PROP_KEY, GAME_MODE_PROP_KEY, AI_PROP_KEY };
            //roomOptions.CustomRoomProperties = new Hashtable { { MAP_PROP_KEY, 1 }, { GAME_MODE_PROP_KEY, 0 } };
            EnterRoomParams enterRoomParams = new EnterRoomParams();
            enterRoomParams.RoomOptions = roomOptions;
            loadBalancingClient.OpCreateRoom(enterRoomParams);
        }

//        public void CreateRoom(string roomName)
//        {
//            if (!string.IsNullOrEmpty(roomName))
//            {
//                PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom, EmptyRoomTtl = 0 });
//                MenuManager.Instance.OpenMenu("loading");
//            }
//            else
//            {
//#if DEBUGGING
//                Debug.LogError("Input Field is empty. Room name must not be empty");
//#endif
//            }
//        }
        #endregion

        #region MonoBehaviourPunCallbacks CallBacks
        // below, we implement some callbacks of PUN
        // you can find PUN's callbacks in the class MonoBehaviourPunCallbacks


        /// <summary>
        /// Called after the connection to the master is established and authenticated
        /// </summary>
        //        public override void OnConnectedToMaster()
        //        {
        //            if (isConnecting)
        //            {
        //#if DEBUGGING
        //                Debug.Log("Connected to Master Server: Attempting to Join Lobby");
        //#endif
        //                LogFeedback("Connected to Master");
        //                PhotonNetwork.JoinLobby();
        //            }
        //        }

        //        public override void OnJoinedLobby()
        //        {
        //#if DEBUGGING
        //            Debug.Log("Joined Lobby");
        //#endif

        //            //TODO connect to auth logins
        //            PhotonNetwork.NickName = "Player_" + Random.Range(10, 10000);
        //            MenuManager.Instance.OpenMenu("main");
        //        }

        //        /// <summary>
        //        /// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
        //        /// </summary>
        //        /// <remarks>
        //        /// Most likely all rooms are full or no rooms are available. <br/>
        //        /// </remarks>
        //        public override void OnJoinRandomFailed(short returnCode, string message)
        //        {
        //            LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
        //            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        //            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        //            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
        //        }


        //        /// <summary>
        //        /// Called after disconnecting from the Photon server.
        //        /// </summary>
        //        public override void OnDisconnected(DisconnectCause cause)
        //        {
        //            LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);
        //            Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");

        //            // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
        //            //loaderAnime.StopLoaderAnimation();

        //            isConnecting = false;

        //        }

        //        /// <summary>
        //        /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
        //        /// </summary>
        //        /// <remarks>
        //        /// This method is commonly used to instantiate player characters.
        //        /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
        //        ///
        //        /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.PlayerList.
        //        /// Also, all custom properties should be already available as Room.customProperties. Check Room..PlayerCount to find out if
        //        /// enough players are in the room to start playing.
        //        /// </remarks>
        //        public override void OnJoinedRoom()
        //        {
        //            LogFeedback("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");

        //#if DEBUGGING
        //            Debug.Log("Player Joined Room");
        //#endif
        //            MenuManager.Instance.OpenMenu("room");
        //        }

        //        public override void OnMasterClientSwitched(Player newMasterClient)
        //        {
        //#if DEBUGGING
        //            Debug.Log("Master Client Switched: " + newMasterClient.ToString());
        //#endif

        //            RoomPlayersUpdated();
        //        }
        //        public override void OnLeftRoom()
        //        {
        //#if DEBUGGING
        //            Debug.Log("Player Left Room");
        //#endif
        //        }

        //        public override void OnJoinRoomFailed(short returnCode, string msg)
        //        {
        //#if DEBUGGING
        //            Debug.Log("Joining Room Failed: " + msg);
        //#endif
        //            LogFeedback("<color=red>Error Joining Game: Game Does Not Exist!</color>\nreturning to menu");
        //            Invoke("ToMainMenu", 3);
        //        }
        //        public override void OnCreateRoomFailed(short returnCode, string msg)
        //        {
        //#if DEBUGGING
        //            Debug.Log("Creating Room Failed: " + msg);
        //#endif
        //        }

        //        public override void OnRoomListUpdate(List<RoomInfo> roomList) => RoomListUpdated(roomList);
        //        public override void OnPlayerEnteredRoom(Player newPlayer) => RoomPlayersUpdated();
        //        public override void OnPlayerLeftRoom(Player otherPlayer) => RoomPlayersUpdated();

        #endregion

        #region Network Callbacks
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.LogWarning("playerjonied");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
#if DEBUGGING
            Debug.Log("<color=yellow>Connected To Server</color>");
#endif
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {

            Debug.LogWarning("Joined Room");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data)
        {
            
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
            
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            
        }
        #endregion

        #region Matchmaking Callbacks
        void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
        {
            throw new System.NotImplementedException();
        }

        void IMatchmakingCallbacks.OnCreatedRoom()
        {
            Debug.LogWarning("Joined Room");
        }

        void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
        {
            throw new System.NotImplementedException();
        }

        void IMatchmakingCallbacks.OnJoinedRoom()
        {
            Debug.LogWarning("Joined Room");
        }

        void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
        {
            throw new System.NotImplementedException();
        }

        void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
        {
            throw new System.NotImplementedException();
        }

        void IMatchmakingCallbacks.OnLeftRoom()
        {
            throw new System.NotImplementedException();
        }
        #endregion

    }
}