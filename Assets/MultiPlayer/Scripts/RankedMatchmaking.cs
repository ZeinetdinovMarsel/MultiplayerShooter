using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

public class RankedMatchmaking : MonoBehaviourPunCallbacks
{
    private int _searchRange = 100;
    private const int MAX_RANGE = 500;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void StartRankedSearch()
    {
        _searchRange = 100;
        TryJoinRoom();
    }

    private void TryJoinRoom()
    {
        int myMMR = PlayerMMR.MMR;

        ExitGames.Client.Photon.Hashtable expectedProps = new ExitGames.Client.Photon.Hashtable
        {
            { "MMR_MIN", myMMR - _searchRange },
            { "MMR_MAX", myMMR + _searchRange }
        };

        bool joinStarted = PhotonNetwork.JoinRandomRoom(expectedProps, 0);

        if (!joinStarted)
            CreateRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    private void CreateRoom()
    {
        int myMMR = PlayerMMR.MMR;

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "MMR_MIN", myMMR - _searchRange },
                { "MMR_MAX", myMMR + _searchRange }
            },
            CustomRoomPropertiesForLobby = new string[] { "MMR_MIN", "MMR_MAX" }
        };

        PhotonNetwork.CreateRoom(null, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Присоединился в комнату рейтинга");

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} присоединился");
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
}
