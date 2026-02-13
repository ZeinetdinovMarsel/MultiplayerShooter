using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

public class RankedMatchmaking : MonoBehaviourPunCallbacks
{
    private int _searchRange = 100;
    private const int MAX_RANGE = 500;

    public void StartRankedSearch()
    {
        TryJoinRoom();
    }

    private void TryJoinRoom()
    {
        int myMMR = PlayerMMR.MMR;

        string sql = $"MMR_MIN <= {myMMR} AND MMR_MAX >= {myMMR}";

        PhotonNetwork.JoinRandomRoom(
            null,
            0,
            MatchmakingMode.FillRoom,
            TypedLobby.Default,
            sql
        );
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    private void CreateRoom()
    {
        int myMMR = PlayerMMR.MMR;

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;

        Hashtable props = new Hashtable
        {
            { "MMR_MIN", myMMR - _searchRange },
            { "MMR_MAX", myMMR + _searchRange }
        };

        options.CustomRoomProperties = props;
        options.CustomRoomPropertiesForLobby = new string[] { "MMR_MIN", "MMR_MAX" };

        PhotonNetwork.CreateRoom(null, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Ranked Room");
    }
}
