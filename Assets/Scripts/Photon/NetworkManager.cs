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
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using PlayFab;
using PlayFab.ClientModels;

#pragma warning disable 649

///TODO change to use singleton pattern with static events for menu systems etc.

namespace CGD.Networking
{
    /// <summary>
    /// Network Manager. Connect, join a random room or create one if none or all full.
    /// </summary>
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        #region Static Events
        public delegate void UpdateRoomListDelegate(List<RoomInfo> roomList);
        public static event UpdateRoomListDelegate OnRoomsUpdated;

        public delegate void NoParamEvent();
        public static event NoParamEvent OnPlayersUpdated;
        #endregion

        #region Private Serializable Fields
        [Tooltip("The Ui Text to inform the user about the connection progress")]
        [SerializeField]
        private TextMeshProUGUI feedbackText;

        [SerializeField] private bool Debugging;
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
        /// List of all rooms
        /// </summary>
        private static List<RoomInfo> roomList = new List<RoomInfo>();

        #endregion

        #region MonoBehaviour CallBacks
        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = false;


            if(!PhotonNetwork.IsConnected)
            {
                LogFeedback("Connecting...");

                // #Critical, we must first and foremost connect to Photon Online Server.
                isConnecting = true;
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = this.gameVersion;
            }
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
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            PhotonNetwork.RaiseEvent(GameSettings.PunLoadScene, 1, raiseEventOptions, SendOptions.SendReliable);

            RoomProperties.SetGameStarted(true);
            //PhotonNetwork.LoadLevel(1);
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

        private static void CreateDebugRoom() 
        {
            PhotonNetwork.CreateRoom("DEBUG_ROOM", new RoomOptions
            {
                MaxPlayers = 1,
                EmptyRoomTtl = 0,
                CustomRoomProperties = RoomProperties.CreateCustomRoomProperties(false, "Debug Room"),
                CustomRoomPropertiesForLobby = RoomProperties.GetLobbyProperties()
            });
            PhotonNetwork.LoadLevel(2);
        }
        public static void CreateRoom(string roomName, int maxPlayers, bool inviteOnly)
        {
            if (!string.IsNullOrEmpty(roomName))
            {
                PhotonNetwork.CreateRoom(RoomProperties.GenerateCode(), new RoomOptions 
                { 
                    MaxPlayers = maxPlayers,//RoomProperties.MaxPlayersPerRoom, 
                    EmptyRoomTtl = 0, 
                    IsVisible = !inviteOnly,
                    CustomRoomProperties = RoomProperties.CreateCustomRoomProperties(false, roomName),
                    CustomRoomPropertiesForLobby = RoomProperties.GetLobbyProperties()
                });

                MenuManager.Instance.OpenMenu("loading");
            }
            else
            {
#if DEBUGGING
                Debug.LogError("Input Field is empty. Room name must not be empty");
#endif
            }
        }

        public static void JoinPrivateRoom(string roomCode)
        {
//            if (roomList == null)
//            {
//#if UNITY_EDITOR && DEBUGGING
//                Debug.LogWarning("No Rooms Found");
//#endif
//            }

            PhotonNetwork.JoinRoom(roomCode);

            //foreach (var roomInfo in roomList)
            //{
            //    if (roomInfo.CustomProperties.TryGetValue(RoomProperties.RoomKey, out object RoomKey) && (string)RoomKey == roomCode)
            //    {
            //        JoinRoom(roomInfo);
            //        return;
            //    }
            //}

//#if UNITY_EDITOR && DEBUGGING
//            Debug.LogWarning("No Rooms Found");
//#endif
        }




        public static void JoinRoom(RoomInfo roomInfo)
        {
            if (roomInfo.CustomProperties.TryGetValue(RoomProperties.GameStarted, out object GameStarted) && (bool)GameStarted)
            {
                PlayerPrefs.SetString(RoomProperties.RoomName, roomInfo.Name);
                SceneManager.LoadScene(1);
            }
            else
            {
                PhotonNetwork.JoinRoom(roomInfo.Name);
                MenuManager.Instance.OpenMenu("loading");
            }
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

        #region MonoBehaviourPunCallbacks CallBacks
        // below, we implement some callbacks of PUN
        // you can find PUN's callbacks in the class MonoBehaviourPunCallbacks


        /// <summary>
        /// Called after the connection to the master is established and authenticated
        /// </summary>
        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
#if DEBUGGING
                Debug.Log("Connected to Master Server: Attempting to Join Lobby");
#endif
                LogFeedback("Connected to Master");
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnJoinedLobby()
        {

#if DEBUGGING
            Debug.Log("Joined Lobby");
            if (Debugging) { CreateDebugRoom(); }
#endif

            //TODO connect to auth logins
            InitialiseUser();
            MenuManager.Instance.OpenMenu("main");
            //GotoLobbyScene();

        }

        ///// <summary>
        ///// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
        ///// </summary>
        ///// <remarks>
        ///// Most likely all rooms are full or no rooms are available. <br/>
        ///// </remarks>
        //public override void OnJoinRandomFailed(short returnCode, string message)
        //{
        //    LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
        //    Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        //    // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        //    PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
        //}


        /// <summary>
        /// Called after disconnecting from the Photon server.
        /// </summary>
        public override void OnDisconnected(DisconnectCause cause)
        {
            LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);
            Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");

            // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
            //loaderAnime.StopLoaderAnimation();

            isConnecting = false;

        }

        /// <summary>
        /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
        /// </summary>
        /// <remarks>
        /// This method is commonly used to instantiate player characters.
        /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
        ///
        /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.PlayerList.
        /// Also, all custom properties should be already available as Room.customProperties. Check Room..PlayerCount to find out if
        /// enough players are in the room to start playing.
        /// </remarks>
        public override void OnJoinedRoom()
        {
            LogFeedback("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");

#if DEBUGGING
            Debug.Log("Player Joined Room");
#endif


            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(RoomProperties.GameStarted, out object GameStarted) && (bool)GameStarted)
            {
                PlayerPrefs.SetString(RoomProperties.RoomName, PhotonNetwork.CurrentRoom.Name);
                SceneManager.LoadScene(1);
            }
            else
            {
                PhotonNetwork.AutomaticallySyncScene = true;
                MenuManager.Instance.OpenMenu("room");
            }
            
            
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
#if DEBUGGING
            Debug.Log("Master Client Switched: " + newMasterClient.ToString());
#endif

            OnPlayersUpdated();
        }
        public override void OnLeftRoom()
        {
#if DEBUGGING
            Debug.Log("Player Left Room");
#endif
        }

        public override void OnJoinRoomFailed(short returnCode, string msg)
        {
#if DEBUGGING
            Debug.Log("Joining Room Failed: " + msg);
#endif
            LogFeedback("<color=red>Error Joining Game: Game Does Not Exist!</color>\nreturning to menu");
            Invoke("ToMainMenu", 3);
        }
        public override void OnCreateRoomFailed(short returnCode, string msg)
        {
#if DEBUGGING
            Debug.Log("Creating Room Failed: " + msg);
#endif
        }

        public override void OnRoomListUpdate(List<RoomInfo> rooms)
        {
            roomList = rooms;
            OnRoomsUpdated?.Invoke(rooms);
        }
        public override void OnPlayerEnteredRoom(Player newPlayer) => OnPlayersUpdated?.Invoke();
        public override void OnPlayerLeftRoom(Player otherPlayer) => OnPlayersUpdated?.Invoke();
        #endregion


        #region PlayFab
        private void InitialiseUser() 
        {
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                var request = new GetAccountInfoRequest();

                PlayFabClientAPI.GetAccountInfo(request, x =>
                {
                    PhotonNetwork.NickName = x.AccountInfo.Username;
                }
                ,OnGetAccountError);
            }
            else 
            {
                PhotonNetwork.NickName = "Player_" + Random.Range(10, 10000);
            }
        }

        private void OnGetAccountError(PlayFabError result)
        {
            PhotonNetwork.NickName = "Player_" + Random.Range(10, 10000);
        }

        #endregion
    }
}