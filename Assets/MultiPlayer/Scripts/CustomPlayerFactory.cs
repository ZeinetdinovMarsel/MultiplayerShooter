using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class CustomPlayerFactory : IFactory<Vector3, Quaternion, GameObject>
{
    private readonly DiContainer _container;
    private readonly GameObject _playerPrefab;

    public CustomPlayerFactory(
        DiContainer container,
        [Inject(Id = "PlayerPrefab")] GameObject playerPrefab)
    {
        _container = container;
        _playerPrefab = playerPrefab;
    }

    public GameObject Create(Vector3 position, Quaternion rotation)
    {
        GameObject obj = PhotonNetwork.Instantiate(_playerPrefab.name, position, rotation);

        _container.InjectGameObject(obj);

        return obj;
    }

}