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

    public void SpawnPlayer()
    {
        _playerFactory.Create(_spawnPoint.position, _spawnPoint.rotation);
    }

    [PunRPC]
    public void RPC_RequestRespawn(PhotonMessageInfo info)
    {
        GameObject playerObj = info.Sender.TagObject as GameObject;
        if (playerObj != null)
        {
            var health = playerObj.GetComponent<Health>();
            if (health != null)
            {
                health.Respawn(_spawnPoint.position, _spawnPoint.rotation);
            }
        }
    }

}
