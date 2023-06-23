using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Room : MonoBehaviour
{
 
    public TextMeshProUGUI roomName;
    public Button joinButton;

    public void Start(){
        joinButton.onClick.AddListener(onClickJoin);
    }
    public void SetRoomName(string _newText)
    {
        roomName.text = _newText;
    }

    public void onClickJoin(){
        UIManager.instance.joinRoomInLobby(roomName.text);
    }
}
