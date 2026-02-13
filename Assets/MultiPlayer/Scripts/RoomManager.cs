using Photon.Pun;
using UnityEngine;
using Zenject;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Inject] private IFactory<Vector3, Quaternion, GameObject> _playerFactory;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom(
            "test",
            new Photon.Realtime.RoomOptions { MaxPlayers = 8 },
            null
        );
    }

    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Vector3 spawnPos = new Vector3(0, 1, 0);
        Quaternion spawnRot = Quaternion.identity;
        _playerFactory.Create(spawnPos, spawnRot);
    }

}
