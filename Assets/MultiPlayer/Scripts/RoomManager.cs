using Photon.Pun;
using UnityEngine;
using Zenject;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform _spawnPoint;
    
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

    private void SpawnPlayer()
    {
        _playerFactory.Create(_spawnPoint.position, _spawnPoint.rotation);
    }
}
