using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

 public class PhotonConnector : MonoBehaviourPunCallbacks
    {
        [SerializeField] private bool _isLogging;
        
        private static PhotonConnector _instance;
        public static PhotonConnector Instance => _instance ??= FindObjectOfType<PhotonConnector>();
        
        private bool _isConnecting;
        private bool _isMaster;
        private string _roomName;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Log(string message)
        {
            if (!_isLogging)
            {
                return;
            }
            
            Debug.LogError(message);
            
        }

        public void Connect(bool isMaster, string roomName)
        {
            _isConnecting = true;
            _isMaster = isMaster;
            _roomName = roomName;
            PhotonNetwork.OfflineMode = false;

            if (PhotonNetwork.IsConnected)
            { 
                return;
            }
            
            PhotonNetwork.NetworkingClient.AppId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
            
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            if (!_isConnecting)
            {
                return;
            }
            
            Log($"Connected to master");
            PhotonNetwork.JoinLobby();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            _isConnecting = false;
        }

        public override void OnJoinedLobby()
        {
            Log($"Joined lobby");

            if (_isMaster)
            {
                PhotonNetwork.JoinOrCreateRoom(_roomName,
                    new RoomOptions
                    {
                        MaxPlayers = 4,
                        CleanupCacheOnLeave = true,
                        PublishUserId = true,
                    },null);
            }
        }

        public override void OnJoinedRoom()
        {
            Log($"Joined room {PhotonNetwork.CurrentRoom.Name}");
            
            //BrodskyEvents.OnMultiplayerConnected();
        }

        public override void OnCreatedRoom()
        {
            Log($"Room {_roomName} created");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Log($"Create room failed: {message}"); 
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Log($"Join room failed: {message}");
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Log($"Room list updated");

            foreach (RoomInfo roomInfo in roomList)
            {
                Log($"   - Room {roomInfo.Name}");

                if (roomInfo.Name == _roomName)
                {
                    PhotonNetwork.JoinRoom(_roomName);
                    return;
                }
            }
        }

        public override void OnPlayerEnteredRoom(Player player)
        {
            Log($"Player {player.ActorNumber} entered room");
                 //TestDebug.Instance.ShowText("actor number" +  player.ActorNumber);
          
            if (/*BrodskyShow.IsStarted &&*/ PhotonNetwork.IsMasterClient)
            {
               
                 int[] targetActors = new int[PhotonNetwork.CurrentRoom.PlayerCount];
                for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
                {
                    targetActors[i] = PhotonNetwork.CurrentRoom.Players[i].ActorNumber;
                }
                
               // MultiplayerEvents.SendProgress(targetActors, BrodskyShow.CurrentIndex, BrodskyShow.CurrentTime);
                return;
            }

            if (_isMaster)
            {
               // BrodskyEvents.OnPlayersUpdated(PhotonNetwork.CurrentRoom.PlayerCount);
            }
        }

        public override void OnPlayerLeftRoom(Player player)
        {
            Log($"Player {player.ActorNumber} left room");
            
            if (_isMaster)
            {
                //BrodskyEvents.OnPlayersUpdated(PhotonNetwork.CurrentRoom.PlayerCount);
            }
        }

        public void EndSession()
        {
           
                PhotonNetwork.Disconnect();
            
        }
        public override void OnMasterClientSwitched(Player player)
        {
            Log($"Master client switched to {player.ActorNumber} {player.NickName}");
        }
    }
    
