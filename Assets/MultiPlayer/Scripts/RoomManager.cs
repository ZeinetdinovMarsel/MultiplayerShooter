using Photon.Pun;
using UnityEngine;
using Zenject;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Inject] private IFactory<Vector3, Quaternion, GameObject> _playerFactory;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
    }

    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-2f, 2f), 1, Random.Range(-2f, 2f));
        Quaternion spawnRot = Quaternion.identity;

        _playerFactory.Create(spawnPos, spawnRot);
    }
}
