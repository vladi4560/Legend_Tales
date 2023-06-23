using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;
    public GameObject lobbyCanvas;
    public GameObject loadingCanvas;
    public GameObject WaitingCanvas;
    public GameObject menuCanvas;
    public PhotonView view;

    #region Unity Methods
    private void Start()
    {
        //Make it a Singleton
        instance = this;
        //DontDestroyOnLoad(gameObject);
        // Connects to Photon master servers
        //PhotonNetwork.ConnectUsingSettings();
        view = GetComponent<PhotonView>();
    }

    #endregion

    public void startServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    #region Photon Callbacks


    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");

        // Automatically joins a lobby after connecting to the master server
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        // Once we've joined the lobby, we want to start the matchmaking process by joining a random room
        //PhotonNetwork.JoinRandomRoom();
        //SceneManager.LoadScene("GamePlay");
        loadingCanvas.SetActive(false);
        lobbyCanvas.SetActive(true);
    }

    // This callback method is called when the attempt to join a random room has failed
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // Debug.Log("Failed to join a random room");

        // // If we failed to join a random room, we may need to create one ourselves.
        // var roomOptions = new RoomOptions() { MaxPlayers = 4 };
        // PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a Room");

        if (PhotonNetwork.IsMasterClient)
        {
            lobbyCanvas.SetActive(false);
            WaitingCanvas.SetActive(true);
            return;
        }
        SceneManager.UnloadSceneAsync(1);
        PhotonNetwork.LoadLevel(2);
        // Now that we've joined a room, we'd like to spawn in our player.
        // We'll spawn the player at (0, 0, 0) for simplicity.
        //PhotonNetwork.Instantiate("PlayerPrefabName", new Vector3(0, 0, 0), Quaternion.identity);

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Other players joined the room.");
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1 && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(2);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UIManager.instance.UpdateRoomList(roomList);
    }


    public override void OnLeftRoom()
    {

        //PhotonNetwork.LeaveRoom();
        
    }

    // public override void OnLeftLobby()
    // {
    //     PhotonNetwork.LeaveLobby();
    //     PhotonNetwork.Disconnect();
    //     lobbyCanvas.SetActive(false);
    //     menuCanvas.SetActive(true);
    // }
    #endregion


    public void CreateRoom(string roomName)
    {
        bool b = PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2 });
    }

    public void JoinRoom(string roomName)
    {
        bool b = PhotonNetwork.JoinRoom(roomName);
    }
}