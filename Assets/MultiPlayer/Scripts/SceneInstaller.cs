using Photon.Pun;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private RankedMatchmaking _rankedMatchmaking;
    [SerializeField] private RankedGameManager _rankedGameManager;
    public override void InstallBindings()
    {
        Container.Bind<RankedMatchmaking>()
            .FromInstance(_rankedMatchmaking)
            .AsSingle();

        Container.Bind<RankedGameManager>()
         .FromInstance(_rankedGameManager)
         .AsSingle();



        Container.Bind<IFactory<Vector3, Quaternion, GameObject>>()
            .To<CustomPlayerFactory>()
            .AsSingle();

    }
}
