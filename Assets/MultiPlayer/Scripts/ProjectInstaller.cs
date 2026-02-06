using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private GameObject _playerPrefab;

    public override void InstallBindings()
    {
        Container.Bind<GameObject>()
            .WithId("PlayerPrefab")
            .FromInstance(_playerPrefab)
            .AsCached();

    }
}