using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private CinemachineCamera _playerCamera;

    public override void InstallBindings()
    {
        Container.Bind<Transform>().WithId("PlayerTransform").FromInstance(_playerTransform).AsCached();
        Container.Bind<CinemachineCamera>().WithId("PlayerFPCamera").FromInstance(_playerCamera).AsSingle();
        Container.Bind<PlayerInputManager>().FromInstance(_playerInputManager).AsSingle();
    }
}
